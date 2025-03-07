// <copyright file="DebuggerTestHelper.cs" company="Datadog">
// Unless explicitly stated otherwise all files in this repository are licensed under the Apache 2 License.
// This product includes software developed at Datadog (https://www.datadoghq.com/). Copyright 2017 Datadog, Inc.
// </copyright>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using Datadog.Trace.Debugger.Configurations.Models;
using Datadog.Trace.Pdb;
using Datadog.Trace.TestHelpers;
using FluentAssertions;
using Samples.Probes.TestRuns;
using Xunit;

namespace Datadog.Trace.Debugger.IntegrationTests.Helpers;

internal static class DebuggerTestHelper
{
    public static IEnumerable<object[]> AllTestDescriptions()
    {
        var assembly = typeof(IRun).Assembly;
        var isOptimized = IsOptimized(assembly);
        var testTypes = assembly.GetTypes()
                                .Where(
                                     t => t.GetInterface(nameof(IRun)) != null ||
                                          t.GetInterface(nameof(IAsyncRun)) != null);

        foreach (var testType in testTypes)
        {
            yield return new object[] { new ProbeTestDescription() { IsOptimized = isOptimized, TestType = testType } };
        }
    }

    public static ProbeTestDescription SpecificTestDescription(Type type)
    {
        var assembly = type.Assembly;
        var isOptimized = IsOptimized(assembly);

        return new ProbeTestDescription() { IsOptimized = isOptimized, TestType = type };
    }

    public static Type FirstSupportedProbeTestType(string framework)
    {
        var type = typeof(IRun)
                  .Assembly.GetTypes()
                  .Where(t => t.GetInterface(nameof(IRun)) != null)
                  .FirstOrDefault(t => GetAllProbes(t, framework, unlisted: false, new DeterministicGuidGenerator()).Any());

        if (type == null)
        {
            throw new SkipException("No supported test types found.");
        }

        return type;
    }

    internal static DebuggerSampleProcessHelper StartSample(TestHelper helper, MockTracerAgent agent, string testName)
    {
        var listenPort = TcpPortProvider.GetOpenPort();

        var localHost = RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ? "http://127.0.0.1" : "http://localhost";
        var listenUrl = $"{localHost}:{listenPort}/";

        var process = helper.StartSample(agent, $"--test-name {testName} --listen-url {listenUrl}", string.Empty, aspNetCorePort: 5000);
        var processHelper = new DebuggerSampleProcessHelper(listenUrl, process);

        return processHelper;
    }

    internal static int CalculateExpectedNumberOfSnapshots(ProbeAttributeBase[] probes)
    {
        return probes.Aggregate(0, (accuNumOfSnapshots, next) => accuNumOfSnapshots + next.ExpectedNumberOfSnapshots);
    }

    internal static bool IsMetricProbe(ProbeAttributeBase probeAttr)
    {
        return probeAttr is MetricMethodProbeTestDataAttribute or MetricLineProbeTestDataAttribute;
    }

    internal static bool IsSpanProbe(ProbeAttributeBase probeAttr)
    {
        return probeAttr is SpanOnMethodProbeTestDataAttribute;
    }

    internal static bool IsLogProbe(ProbeAttributeBase probeAttr)
    {
        return probeAttr is LogMethodProbeTestDataAttribute or LogLineProbeTestDataAttribute;
    }

    internal static (ProbeAttributeBase ProbeTestData, ProbeDefinition Probe)[] GetAllProbes(Type type, string targetFramework, bool unlisted, DeterministicGuidGenerator guidGenerator)
    {
        return GetAllMethodProbes(type, targetFramework, unlisted, guidGenerator)
              .Concat(GetAllLineProbes(type, targetFramework, unlisted, guidGenerator))
              .ToArray();
    }

    internal static IEnumerable<(ProbeAttributeBase ProbeTestData, ProbeDefinition Probe)> GetAllLineProbes(Type type, string targetFramework, bool unlisted, DeterministicGuidGenerator guidGenerator)
    {
        var snapshotLineProbes = type.GetCustomAttributes<LineProbeTestDataAttribute>()
                                     .Where(att => att.Skip == false && att.Unlisted == unlisted && att.SkipOnFrameworks.Contains(targetFramework) == false)
                                     .Select(att => (att.As<ProbeAttributeBase>(), CreateLogLineProbe(type, att, guidGenerator)))
                                     .ToArray();

        return snapshotLineProbes;
    }

    internal static IEnumerable<(ProbeAttributeBase ProbeTestData, ProbeDefinition Probe)> GetAllMethodProbes(Type type, string targetFramework, bool unlisted, DeterministicGuidGenerator guidGenerator)
    {
        var snapshotMethodProbes = GetAllTestMethods<MethodProbeTestDataAttribute>(type, targetFramework, unlisted)
           .Select(m =>
            {
                var probes = new List<(ProbeAttributeBase ProbeTestData, ProbeDefinition Probe)>();
                var testAttribute = m.GetCustomAttribute<MethodProbeTestDataAttribute>().As<ProbeAttributeBase>();

                if (IsMetricProbe(testAttribute))
                {
                    probes.Add((testAttribute, CreateMetricMethodProbe(m, guidGenerator)));
                }
                else if (IsSpanProbe(testAttribute))
                {
                    probes.Add((testAttribute, CreateSpanMethodProbe(m, guidGenerator)));
                }
                else if (IsLogProbe(testAttribute))
                {
                    probes.Add((testAttribute, CreateLogMethodProbe(m, guidGenerator)));
                }
                else
                {
                    throw new InvalidOperationException($"Unknown probe type: {testAttribute.GetType()}");
                }

                return probes;
            });

        return snapshotMethodProbes.SelectMany(p => p);
    }

    internal static T CreateBasicProbe<T>(string probeId)
    where T : ProbeDefinition, new()
    {
        return new T
        {
            Id = probeId,
            Language = TracerConstants.Language,
        };
    }

    internal static ProbeDefinition CreateDefaultLogProbe(string typeName, string methodName, DeterministicGuidGenerator guidGenerator, MethodProbeTestDataAttribute probeTestData = null)
    {
        return CreateBasicProbe<LogProbe>(probeTestData?.ProbeId ?? guidGenerator.New().ToString()).WithSampling().WithDefaultTemplate().WithCapture(probeTestData?.CaptureSnapshot).WithMethodWhere(typeName, methodName, probeTestData);
    }

    private static ProbeDefinition WithMethodWhere(this ProbeDefinition snapshot, string typeName, string methodName, MethodProbeTestDataAttribute probeTestData = null)
    {
        var @where = new Where
        {
            TypeName = typeName,
            MethodName = methodName
        };

        if (probeTestData != null)
        {
            var signature = probeTestData.ReturnTypeName;
            if (probeTestData.ParametersTypeName?.Any() == true)
            {
                signature += "," + string.Join(",", probeTestData.ParametersTypeName);
            }

            @where.Signature = signature;
        }

        snapshot.Where = where;
        return snapshot;
    }

    private static ProbeDefinition WithLineProbeWhere(this ProbeDefinition snapshot, Type type, LineProbeTestDataAttribute line)
    {
        using var reader = DatadogPdbReader.CreatePdbReader(type.Assembly);
        var symbolMethod = reader.ReadMethodSymbolInfo(type.GetMethods().First().MetadataToken);
        var filePath = symbolMethod.SequencePoints.First().Document.URL;
        var where = new Where { SourceFile = filePath, Lines = new[] { line.LineNumber.ToString() } };
        snapshot.Where = where;
        return snapshot;
    }

    private static LogProbe WithCapture(this LogProbe snapshot, bool? captureSnapshot)
    {
        if (!captureSnapshot.HasValue || !captureSnapshot.Value)
        {
            snapshot.CaptureSnapshot = false;
            return snapshot;
        }

        var capture = new Capture
        {
            MaxCollectionSize = 1000,
            MaxFieldCount = 10000,
            MaxFieldDepth = 3,
            MaxLength = int.MaxValue,
            MaxReferenceDepth = 3
        };

        snapshot.CaptureSnapshot = true;
        snapshot.Capture = capture;
        return snapshot;
    }

    private static LogProbe WithWhen(this LogProbe snapshot, ProbeAttributeBase att)
    {
        if (att == null || string.IsNullOrEmpty(att.ConditionJson))
        {
            return snapshot;
        }

        snapshot.When = new SnapshotSegment(string.Empty, att.ConditionJson, null);
        snapshot.EvaluateAt = ParseEnum<EvaluateAt>(att.EvaluateAt);
        return snapshot;
    }

    private static LogProbe WithSampling(this LogProbe snapshot, double snapshotsPerSeconds = 1000000)
    {
        snapshot.Sampling = new Configurations.Models.Sampling { SnapshotsPerSecond = snapshotsPerSeconds };
        return snapshot;
    }

    private static LogProbe WithDefaultTemplate(this LogProbe snapshot)
    {
        if (snapshot.Segments != null)
        {
            return snapshot;
        }

        snapshot.Template = "Test {1}";
        var json = @"{
    ""Ignore"": ""1""
}";
        snapshot.Segments = new SnapshotSegment[] { new(null, null, "Test"), new("1", json, null) };
        snapshot.EvaluateAt = EvaluateAt.Entry;
        return snapshot;
    }

    private static LogProbe WithTemplate(this LogProbe snapshot, ProbeAttributeBase att)
    {
        if (att == null || (string.IsNullOrEmpty(att.TemplateJson) && string.IsNullOrEmpty(att.TemplateStr)))
        {
            return snapshot.WithDefaultTemplate();
        }

        var segments = new List<SnapshotSegment>();

        if (att.TemplateStr != null)
        {
            segments.Add(new SnapshotSegment(null, null, att.TemplateStr));
        }

        if (att.TemplateJson != null)
        {
            segments.Add(new SnapshotSegment(string.Empty, att.TemplateJson, null));
        }

        snapshot.Segments = segments.ToArray();
        snapshot.EvaluateAt = ParseEnum<EvaluateAt>(att.EvaluateAt ?? "Exit");
        return snapshot;
    }

    private static MetricProbe WithMetric(this MetricProbe probe, MetricMethodProbeTestDataAttribute probeTestData)
    {
        probe.Value = new SnapshotSegment(null, probeTestData.MetricJson, string.Empty);
        probe.Kind = ParseEnum<MetricKind>(probeTestData.MetricKind);
        probe.MetricName = probeTestData.MetricName;
        return probe;
    }

    private static ProbeDefinition CreateMetricMethodProbe(MethodBase method, DeterministicGuidGenerator guidGenerator)
    {
        var probeTestData = GetProbeTestData(method, out var typeName) as MetricMethodProbeTestDataAttribute;

        if (probeTestData == null || string.IsNullOrEmpty(probeTestData.MetricKind))
        {
            throw new InvalidOperationException("This probe attribute has no metric information");
        }

        return CreateBasicProbe<MetricProbe>(probeTestData.ProbeId ?? guidGenerator.New().ToString()).WithMetric(probeTestData).WithMethodWhere(typeName, method.Name, probeTestData);
    }

    private static ProbeDefinition CreateSpanMethodProbe(MethodBase method, DeterministicGuidGenerator guidGenerator)
    {
        var probeTestData = GetProbeTestData(method, out var typeName);
        if (probeTestData == null)
        {
            throw new InvalidOperationException("Probe attribute is null");
        }

        return CreateBasicProbe<SpanProbe>(probeTestData.ProbeId ?? guidGenerator.New().ToString()).WithMethodWhere(typeName, method.Name, probeTestData);
    }

    private static bool IsOptimized(Assembly assembly)
    {
        var debuggableAttribute = (DebuggableAttribute)Attribute.GetCustomAttribute(assembly, typeof(DebuggableAttribute));
        var isOptimized = !debuggableAttribute.IsJITOptimizerDisabled;
        return isOptimized;
    }

    private static ProbeDefinition CreateLogMethodProbe(MethodBase method, DeterministicGuidGenerator guidGenerator)
    {
        var probeTestData = GetProbeTestData(method, out var typeName);
        if (probeTestData == null)
        {
            throw new InvalidOperationException("Probe attribute is null");
        }

        return CreateBasicProbe<LogProbe>(probeTestData.ProbeId ?? guidGenerator.New().ToString()).WithCapture(probeTestData.CaptureSnapshot).WithSampling().WithTemplate(probeTestData).WithWhen(probeTestData).WithMethodWhere(typeName, method.Name, probeTestData);
    }

    private static MethodProbeTestDataAttribute GetProbeTestData(MethodBase method, out string typeName)
    {
        var probeTestData = method.GetCustomAttribute<MethodProbeTestDataAttribute>();
        if (probeTestData == null)
        {
            throw new Xunit.Sdk.SkipException($"{nameof(MethodProbeTestDataAttribute)} has not found for method: {method.DeclaringType?.FullName}.{method.Name}");
        }

        typeName = probeTestData.UseFullTypeName ? method.DeclaringType?.FullName : method.DeclaringType?.Name;

        if (typeName == null)
        {
            throw new Xunit.Sdk.SkipException($"{nameof(CreateLogMethodProbe)} failed in getting type name for method: {method.Name}");
        }

        return probeTestData;
    }

    private static ProbeDefinition CreateLogLineProbe(Type type, LineProbeTestDataAttribute line, DeterministicGuidGenerator guidGenerator)
    {
        return CreateBasicProbe<LogProbe>(line?.ProbeId ?? guidGenerator.New().ToString()).WithCapture(line?.CaptureSnapshot).WithSampling().WithTemplate(line).WithWhen(line).WithLineProbeWhere(type, line);
    }

    private static IEnumerable<MethodBase> GetAllTestMethods<T>(Type type, string targetFramework, bool unlisted)
        where T : ProbeAttributeBase
    {
        const BindingFlags allMask =
            BindingFlags.Public |
            BindingFlags.NonPublic |
            BindingFlags.Static |
            BindingFlags.Instance;

        return type.GetNestedTypes(allMask)
                   .SelectMany(
                        nestedType =>
                            nestedType.GetMethods(allMask | BindingFlags.DeclaredOnly)
                                      .Concat(nestedType.GetConstructors(allMask | BindingFlags.DeclaredOnly).As<IEnumerable<MethodBase>>()))
                   .Concat(
                        type.GetMethods(allMask | BindingFlags.DeclaredOnly)
                            .Concat(type.GetConstructors(allMask | BindingFlags.DeclaredOnly).As<IEnumerable<MethodBase>>()))
                   .As<IEnumerable<MethodBase>>()
                   .Where(
                        m =>
                        {
                            var att = m.GetCustomAttribute<T>();
                            return att?.Skip == false && att.Unlisted == unlisted && att.SkipOnFrameworks.Contains(targetFramework) == false;
                        });
    }

    private static T ParseEnum<T>(string enumValue)
        where T : struct
    {
#if NETFRAMEWORK
        return (T)Enum.Parse(typeof(T), enumValue, true);
#else
        return Enum.Parse<T>(enumValue, true);
#endif
    }
}
