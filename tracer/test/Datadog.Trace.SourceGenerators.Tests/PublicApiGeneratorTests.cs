﻿// <copyright file="PublicApiGeneratorTests.cs" company="Datadog">
// Unless explicitly stated otherwise all files in this repository are licensed under the Apache 2 License.
// This product includes software developed at Datadog (https://www.datadoghq.com/). Copyright 2017 Datadog, Inc.
// </copyright>

using Datadog.Trace.SourceGenerators.PublicApi;
using Datadog.Trace.SourceGenerators.PublicApi.Diagnostics;
using FluentAssertions;
using FluentAssertions.Execution;
using Xunit;

namespace Datadog.Trace.SourceGenerators.Tests;

public class PublicApiGeneratorTests
{
    [Theory]
    [InlineData("")]
    [InlineData("const ")]
    [InlineData("readonly ")]
    public void CanGenerateReadOnlyProperty(string modifier)
    {
        var input = $$"""
            using Datadog.Trace.Configuration;
            using Datadog.Trace.SourceGenerators;
            using Datadog.Trace.Telemetry.Metrics;

            #nullable enable
            namespace MyTests.TestMetricNameSpace;
            public partial class TestSettings
            {
                /// <summary>
                /// Gets the default environment name applied to all spans.
                /// </summary>
                /// <seealso cref="ConfigurationKeys.Environment"/>
                [GeneratePublicApi(PublicApiUsage.Environment_Get)]
                internal {{modifier}}string? _Environment;
            }
            """;

        const string expected = """
            // <auto-generated/>
            #nullable enable

            namespace MyTests.TestMetricNameSpace;
            partial class TestSettings
            {
                /// <summary>
                /// Gets the default environment name applied to all spans.
                /// </summary>
                /// <seealso cref="ConfigurationKeys.Environment"/>
                [Datadog.Trace.SourceGenerators.PublicApi]
                public string? Environment
                {
                    get
                    {
                        Datadog.Trace.Telemetry.Metrics.TelemetryMetrics.Instance.Record(
                            (Datadog.Trace.Telemetry.Metrics.PublicApiUsage)0);
                        return _Environment;
                    }
                }
            }
            """;

        var (diagnostics, output) = TestHelpers.GetGeneratedOutput<PublicApiGenerator>(input, GetPublicApiAttribute());
        using var scope = new AssertionScope();
        diagnostics.Should().BeEmpty();
        output.Should().Be(expected);
    }

    [Theory]
    [InlineData("GeneratePublicApi")]
    [InlineData("GeneratePublicApiAttribute")]
    [InlineData("Datadog.Trace.SourceGenerators.GeneratePublicApiAttribute")]
    public void CanGenerateReadWriteProperty(string attributeName)
    {
        var input = $$"""
            using Datadog.Trace.Configuration;
            using Datadog.Trace.SourceGenerators;
            using Datadog.Trace.Telemetry.Metrics;

            #nullable enable
            namespace MyTests.TestMetricNameSpace;
            public partial class TestSettings
            {
                /// <summary>
                /// Gets the default environment name applied to all spans.
                /// </summary>
                /// <seealso cref="ConfigurationKeys.Environment"/>
                [{{attributeName}}(PublicApiUsage.Environment_Get, PublicApiUsage.Environment_Set)]
                internal string? _Environment;
            }
            """;

        const string expected = """
            // <auto-generated/>
            #nullable enable

            namespace MyTests.TestMetricNameSpace;
            partial class TestSettings
            {
                /// <summary>
                /// Gets the default environment name applied to all spans.
                /// </summary>
                /// <seealso cref="ConfigurationKeys.Environment"/>
                [Datadog.Trace.SourceGenerators.PublicApi]
                public string? Environment
                {
                    get
                    {
                        Datadog.Trace.Telemetry.Metrics.TelemetryMetrics.Instance.Record(
                            (Datadog.Trace.Telemetry.Metrics.PublicApiUsage)0);
                        return _Environment;
                    }
                    set
                    {
                        Datadog.Trace.Telemetry.Metrics.TelemetryMetrics.Instance.Record(
                            (Datadog.Trace.Telemetry.Metrics.PublicApiUsage)1);
                        _Environment = value;
                    }
                }
            }
            """;

        var (diagnostics, output) = TestHelpers.GetGeneratedOutput<PublicApiGenerator>(input, GetPublicApiAttribute());
        using var scope = new AssertionScope();
        diagnostics.Should().BeEmpty();
        output.Should().Be(expected);
    }

    [Fact]
    public void CanGenerateMultipleProperties()
    {
        const string input = """
            using Datadog.Trace.Configuration;
            using Datadog.Trace.SourceGenerators;
            using Datadog.Trace.Telemetry.Metrics;

            #nullable enable
            namespace MyTests.TestMetricNameSpace;
            public partial class TestSettings
            {
                /// <summary>
                /// Gets the default environment name applied to all spans.
                /// </summary>
                /// <seealso cref="ConfigurationKeys.Environment"/>
                [GeneratePublicApi(PublicApiUsage.Environment_Get, PublicApiUsage.Environment_Set)]
                internal string? _Environment;

                /// <summary>
                /// Gets the default service name applied to all spans.
                /// </summary>
                /// <seealso cref="ConfigurationKeys.Service"/>
                [GeneratePublicApi(PublicApiUsage.ServiceName_Get)]
                internal HashSet<string> ServiceNameInternal;
            }
            """;

        const string expected = """
            // <auto-generated/>
            #nullable enable

            namespace MyTests.TestMetricNameSpace;
            partial class TestSettings
            {
                /// <summary>
                /// Gets the default environment name applied to all spans.
                /// </summary>
                /// <seealso cref="ConfigurationKeys.Environment"/>
                [Datadog.Trace.SourceGenerators.PublicApi]
                public string? Environment
                {
                    get
                    {
                        Datadog.Trace.Telemetry.Metrics.TelemetryMetrics.Instance.Record(
                            (Datadog.Trace.Telemetry.Metrics.PublicApiUsage)0);
                        return _Environment;
                    }
                    set
                    {
                        Datadog.Trace.Telemetry.Metrics.TelemetryMetrics.Instance.Record(
                            (Datadog.Trace.Telemetry.Metrics.PublicApiUsage)1);
                        _Environment = value;
                    }
                }

                /// <summary>
                /// Gets the default service name applied to all spans.
                /// </summary>
                /// <seealso cref="ConfigurationKeys.Service"/>
                [Datadog.Trace.SourceGenerators.PublicApi]
                public HashSet<string> ServiceName
                {
                    get
                    {
                        Datadog.Trace.Telemetry.Metrics.TelemetryMetrics.Instance.Record(
                            (Datadog.Trace.Telemetry.Metrics.PublicApiUsage)2);
                        return ServiceNameInternal;
                    }
                }
            }
            """;

        var (diagnostics, output) = TestHelpers.GetGeneratedOutput<PublicApiGenerator>(input, GetPublicApiAttribute());
        using var scope = new AssertionScope();
        diagnostics.Should().BeEmpty();
        output.Should().Be(expected);
    }

    [Theory]
    [InlineData("struct")]
    [InlineData("record struct")]
    public void CanNotGenerateForStruct(string type)
    {
        var input = $$"""
            using Datadog.Trace.Configuration;
            using Datadog.Trace.SourceGenerators;
            using Datadog.Trace.Telemetry.Metrics;

            #nullable enable
            namespace MyTests.TestMetricNameSpace;
            public partial {{type}} TestSettings
            {
                /// <summary>
                /// Gets the default environment name applied to all spans.
                /// </summary>
                /// <seealso cref="ConfigurationKeys.Environment"/>
                [GeneratePublicApi(PublicApiUsage.Environment_Get, PublicApiUsage.Environment_Set)]
                internal string? _Environment;
            }
            """;

        var (diagnostics, output) = TestHelpers.GetGeneratedOutput<PublicApiGenerator>(input, GetPublicApiAttribute());
        Assert.Contains(diagnostics, diag => diag.Id == OnlySupportsClassesDiagnostic.Id);
    }

    [Theory]
    [InlineData("const")]
    [InlineData("readonly")]
    public void CanNotGenerateSetterForReadOnlyProperty(string modifier)
    {
        var input = $$"""
            using Datadog.Trace.Configuration;
            using Datadog.Trace.SourceGenerators;
            using Datadog.Trace.Telemetry.Metrics;

            #nullable enable
            namespace MyTests.TestMetricNameSpace;
            public partial class TestSettings
            {
                /// <summary>
                /// Gets the default environment name applied to all spans.
                /// </summary>
                /// <seealso cref="ConfigurationKeys.Environment"/>
                [GeneratePublicApi(PublicApiUsage.Environment_Get, PublicApiUsage.Environment_Set)]
                internal {{modifier}} string? _Environment = "Something";
            }
            """;

        var (diagnostics, output) = TestHelpers.GetGeneratedOutput<PublicApiGenerator>(input, GetPublicApiAttribute());
        Assert.Contains(diagnostics, diag => diag.Id == SetterOnReadonlyFieldDiagnostic.Id);
    }

    [Fact]
    public void ErrorsWhenCantDetermineName()
    {
        var input = """
            using Datadog.Trace.Configuration;
            using Datadog.Trace.SourceGenerators;
            using Datadog.Trace.Telemetry.Metrics;

            #nullable enable
            namespace MyTests.TestMetricNameSpace;
            public partial class TestSettings
            {
                /// <summary>
                /// Gets the default environment name applied to all spans.
                /// </summary>
                /// <seealso cref="ConfigurationKeys.Environment"/>
                [GeneratePublicApi(PublicApiUsage.Environment_Get, PublicApiUsage.Environment_Set)]
                internal string? Environment;
            }
            """;

        var (diagnostics, output) = TestHelpers.GetGeneratedOutput<PublicApiGenerator>(input, GetPublicApiAttribute());
        Assert.Contains(diagnostics, diag => diag.Id == NamingProblemDiagnostic.Id);
    }

    [Fact]
    public void AddsDiagnosticWhenPartialModifierMissing()
    {
        var input = """
            using Datadog.Trace.Configuration;
            using Datadog.Trace.SourceGenerators;
            using Datadog.Trace.Telemetry.Metrics;

            #nullable enable
            namespace MyTests.TestMetricNameSpace;
            public class TestSettings
            {
                /// <summary>
                /// Gets the default environment name applied to all spans.
                /// </summary>
                /// <seealso cref="ConfigurationKeys.Environment"/>
                [GeneratePublicApi(PublicApiUsage.Environment_Get, PublicApiUsage.Environment_Set)]
                internal string? Environment;
            }
            """;

        var (diagnostics, output) = TestHelpers.GetGeneratedOutput<PublicApiGenerator>(input, GetPublicApiAttribute());
        Assert.Contains(diagnostics, diag => diag.Id == PartialModifierIsRequiredDiagnostic.Id);
    }

    private static string GetPublicApiAttribute()
        => """
            namespace Datadog.Trace.Telemetry.Metrics;
            public enum PublicApiUsage 
            {
                Environment_Get,
                Environment_Set,
                ServiceName_Get,
            }
            """;
}
