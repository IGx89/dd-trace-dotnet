// <copyright file="TestHelper.cs" company="Datadog">
// Unless explicitly stated otherwise all files in this repository are licensed under the Apache 2 License.
// This product includes software developed at Datadog (https://www.datadoghq.com/). Copyright 2017 Datadog, Inc.
// </copyright>

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Datadog.Trace.Configuration;
using Datadog.Trace.Debugger.Helpers;
using Datadog.Trace.ExtensionMethods;
using Datadog.Trace.RemoteConfigurationManagement.Protocol;
using Datadog.Trace.RemoteConfigurationManagement.Protocol.Tuf;
using Datadog.Trace.Vendors.Newtonsoft.Json;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace Datadog.Trace.TestHelpers
{
    public abstract class TestHelper : IDisposable
    {
        private HttpClient _client = new();

        protected TestHelper(string sampleAppName, string samplePathOverrides, ITestOutputHelper output)
            : this(new EnvironmentHelper(sampleAppName, typeof(TestHelper), output, samplePathOverrides), output)
        {
        }

        protected TestHelper(string sampleAppName, string samplePathOverrides, ITestOutputHelper output, bool prependSamplesToAppName)
            : this(new EnvironmentHelper(sampleAppName, typeof(TestHelper), output, samplePathOverrides, prependSamplesToAppName: false), output)
        {
        }

        protected TestHelper(string sampleAppName, ITestOutputHelper output)
            : this(new EnvironmentHelper(sampleAppName, typeof(TestHelper), output), output)
        {
        }

        protected TestHelper(EnvironmentHelper environmentHelper, ITestOutputHelper output)
        {
            EnvironmentHelper = environmentHelper;
            Output = output;

            Output.WriteLine($"Platform: {EnvironmentTools.GetPlatform()}");
            Output.WriteLine($"TargetPlatform: {EnvironmentTools.GetTestTargetPlatform()}");
            Output.WriteLine($"Configuration: {EnvironmentTools.GetBuildConfiguration()}");
            Output.WriteLine($"TargetFramework: {EnvironmentHelper.GetTargetFramework()}");
            Output.WriteLine($".NET Core: {EnvironmentHelper.IsCoreClr()}");
            Output.WriteLine($"Native Loader DLL: {EnvironmentHelper.GetNativeLoaderPath()}");

            _client.DefaultRequestHeaders.Add("DD-API-KEY", Environment.GetEnvironmentVariable("DD_LOGGER_DD_API_KEY"));
        }

        public bool SecurityEnabled { get; private set; }

        protected EnvironmentHelper EnvironmentHelper { get; }

        protected string TestPrefix => $"{EnvironmentTools.GetBuildConfiguration()}.{EnvironmentHelper.GetTargetFramework()}";

        protected ITestOutputHelper Output { get; }

        public virtual void Dispose()
        {
        }

        public Process StartDotnetTestSample(MockTracerAgent agent, string arguments, string packageVersion, int aspNetCorePort, string framework = "", bool forceVsTestParam = false)
        {
            // get path to sample app that the profiler will attach to
            string sampleAppPath = EnvironmentHelper.GetTestCommandForSampleApplicationPath(packageVersion, framework);
            if (!File.Exists(sampleAppPath))
            {
                throw new Exception($"application not found: {sampleAppPath}");
            }

            Output.WriteLine($"Starting Application: {sampleAppPath}");
            string testCli = EnvironmentHelper.GetDotNetTest();
            string exec = testCli;
            string appPath = testCli.StartsWith("dotnet") || forceVsTestParam ? $"vstest {sampleAppPath}" : sampleAppPath;
            Output.WriteLine("Executable: " + exec);
            Output.WriteLine("ApplicationPath: " + appPath);
            var process = ProfilerHelper.StartProcessWithProfiler(
                exec,
                EnvironmentHelper,
                agent,
                $"{appPath} {arguments ?? string.Empty}",
                aspNetCorePort: aspNetCorePort,
                processToProfile: exec + ";testhost.exe;testhost.x86.exe");

            Output.WriteLine($"ProcessId: {process.Id}");

            return process;
        }

        public ProcessResult RunDotnetTestSampleAndWaitForExit(MockTracerAgent agent, string arguments = null, string packageVersion = "", string framework = "", bool forceVsTestParam = false)
        {
            var process = StartDotnetTestSample(agent, arguments, packageVersion, aspNetCorePort: 5000, framework: framework, forceVsTestParam: forceVsTestParam);

            using var helper = new ProcessHelper(process);

            process.WaitForExit();
            helper.Drain();
            var exitCode = process.ExitCode;

            Output.WriteLine($"Exit Code: " + exitCode);

            var standardOutput = helper.StandardOutput;

            if (!string.IsNullOrWhiteSpace(standardOutput))
            {
                Output.WriteLine($"StandardOutput:{Environment.NewLine}{standardOutput}");
            }

            var standardError = helper.ErrorOutput;

            if (!string.IsNullOrWhiteSpace(standardError))
            {
                Output.WriteLine($"StandardError:{Environment.NewLine}{standardError}");
            }

            return new ProcessResult(process, standardOutput, standardError, exitCode);
        }

        public Process StartSample(MockTracerAgent agent, string arguments, string packageVersion, int aspNetCorePort, string framework = "", bool? enableSecurity = null, string externalRulesFile = null)
        {
            // get path to sample app that the profiler will attach to
            var sampleAppPath = EnvironmentHelper.GetSampleApplicationPath(packageVersion, framework);
            if (!File.Exists(sampleAppPath))
            {
                throw new Exception($"application not found: {sampleAppPath}");
            }

            Output.WriteLine($"Starting Application: {sampleAppPath}");
            var executable = EnvironmentHelper.IsCoreClr() ? EnvironmentHelper.GetSampleExecutionSource() : sampleAppPath;
            var args = EnvironmentHelper.IsCoreClr() ? $"{sampleAppPath} {arguments ?? string.Empty}" : arguments;

            var process = ProfilerHelper.StartProcessWithProfiler(
                executable,
                EnvironmentHelper,
                agent,
                args,
                aspNetCorePort: aspNetCorePort,
                processToProfile: executable,
                enableSecurity: enableSecurity,
                externalRulesFile: externalRulesFile);

            Output.WriteLine($"ProcessId: {process.Id}");

            return process;
        }

        public ProcessResult RunSampleAndWaitForExit(MockTracerAgent agent, string arguments = null, string packageVersion = "", string framework = "", int aspNetCorePort = 5000)
        {
            var process = StartSample(agent, arguments, packageVersion, aspNetCorePort: aspNetCorePort, framework: framework);
            using var helper = new ProcessHelper(process);

            return WaitForProcessResult(helper);
        }

        public ProcessResult WaitForProcessResult(ProcessHelper helper, int expectedExitCode = 0)
        {
            // this is _way_ too long, but we want to be v. safe
            // the goal is just to make sure we kill the test before
            // the whole CI run times out
            var process = helper.Process;
            var timeoutMs = (int)TimeSpan.FromMinutes(10).TotalMilliseconds;
            var ranToCompletion = process.WaitForExit(timeoutMs) && helper.Drain(timeoutMs / 2);

            var standardOutput = helper.StandardOutput;

            if (!string.IsNullOrWhiteSpace(standardOutput))
            {
                Output.WriteLine($"StandardOutput:{Environment.NewLine}{standardOutput}");
            }

            var standardError = helper.ErrorOutput;

            if (!string.IsNullOrWhiteSpace(standardError))
            {
                Output.WriteLine($"StandardError:{Environment.NewLine}{standardError}");
            }

            if (!ranToCompletion && !process.HasExited)
            {
                var tookMemoryDump = MemoryDumpHelper.CaptureMemoryDump(process);
                process.Kill();
                throw new Exception($"The sample did not exit in {timeoutMs}ms. Memory dump taken: {tookMemoryDump}. Killing process.");
            }

            var exitCode = process.ExitCode;

            Output.WriteLine($"ProcessId: " + process.Id);
            Output.WriteLine($"Exit Code: " + exitCode);

#if NETCOREAPP2_1
            if (exitCode == 139)
            {
                // Segmentation faults are expected on .NET Core because of a bug in the runtime: https://github.com/dotnet/runtime/issues/11885
                throw new SkipException("Segmentation fault on .NET Core 2.1");
            }
#endif
            if (exitCode == 134
             && standardError?.Contains("System.Threading.AbandonedMutexException: The wait completed due to an abandoned mutex") == true
             && standardError?.Contains("Coverlet.Core.Instrumentation.Tracker") == true)
            {
                // Coverlet occasionally throws AbandonedMutexException during clean up
                throw new SkipException("Coverlet threw AbandonedMutexException during cleanup");
            }

            ExitCodeException.ThrowIfNonExpected(exitCode, expectedExitCode);

            return new ProcessResult(process, standardOutput, standardError, exitCode);
        }

        public (Process Process, string ConfigFile) StartIISExpress(MockTracerAgent agent, int iisPort, IisAppType appType, string subAppPath)
        {
            var iisExpress = EnvironmentHelper.GetIisExpressPath();

            var appPool = appType switch
            {
                IisAppType.AspNetClassic => "Clr4ClassicAppPool",
                IisAppType.AspNetIntegrated => "Clr4IntegratedAppPool",
                IisAppType.AspNetCoreInProcess => "UnmanagedClassicAppPool",
                IisAppType.AspNetCoreOutOfProcess => "UnmanagedClassicAppPool",
                _ => throw new InvalidOperationException($"Unknown {nameof(IisAppType)} '{appType}'"),
            };

            var appPath = appType switch
            {
                IisAppType.AspNetClassic => EnvironmentHelper.GetSampleProjectDirectory(),
                IisAppType.AspNetIntegrated => EnvironmentHelper.GetSampleProjectDirectory(),
                IisAppType.AspNetCoreInProcess => EnvironmentHelper.GetSampleApplicationOutputDirectory(),
                IisAppType.AspNetCoreOutOfProcess => EnvironmentHelper.GetSampleApplicationOutputDirectory(),
                _ => throw new InvalidOperationException($"Unknown {nameof(IisAppType)} '{appType}'"),
            };

            var configTemplate = File.ReadAllText("applicationHost.config");

            var newConfig = Path.GetTempFileName();

            var virtualAppSection = subAppPath switch
            {
                null or "" or "/" => string.Empty,
                _ when !subAppPath.StartsWith("/") => throw new ArgumentException("Application path must start with '/'", nameof(subAppPath)),
                _ when subAppPath.EndsWith("/") => throw new ArgumentException("Application path must not end with '/'", nameof(subAppPath)),
                _ => $"<application path=\"{subAppPath}\" applicationPool=\"{appPool}\"><virtualDirectory path=\"/\" physicalPath=\"{appPath}\" /></application>",
            };

            configTemplate = configTemplate
                            .Replace("[PATH]", appPath)
                            .Replace("[PORT]", iisPort.ToString())
                            .Replace("[POOL]", appPool)
                            .Replace("[VIRTUAL_APPLICATION]", virtualAppSection);

            var isAspNetCore = appType == IisAppType.AspNetCoreInProcess || appType == IisAppType.AspNetCoreOutOfProcess;
            if (isAspNetCore)
            {
                var hostingModel = appType == IisAppType.AspNetCoreInProcess ? "inprocess" : "outofprocess";
                configTemplate = configTemplate
                                .Replace("[DOTNET]", EnvironmentHelper.GetDotnetExe())
                                .Replace("[RELATIVE_SAMPLE_PATH]", $".\\{EnvironmentHelper.GetSampleApplicationFileName()}")
                                .Replace("[HOSTING_MODEL]", hostingModel);
            }

            File.WriteAllText(newConfig, configTemplate);

            var args = new[]
                {
                    "/site:sample",
                    $"/config:{newConfig}",
                    "/systray:false",
                    "/trace:info"
                };

            Output.WriteLine($"[webserver] starting {iisExpress} {string.Join(" ", args)}");

            var process = ProfilerHelper.StartProcessWithProfiler(
                iisExpress,
                EnvironmentHelper,
                agent,
                arguments: string.Join(" ", args),
                redirectStandardInput: true,
                processToProfile: appType == IisAppType.AspNetCoreOutOfProcess ? "dotnet.exe" : iisExpress);

            var wh = new EventWaitHandle(false, EventResetMode.AutoReset);

            Task.Factory.StartNew(
                () =>
                {
                    string line;
                    while ((line = process.StandardOutput.ReadLine()) != null)
                    {
                        Output.WriteLine($"[webserver][stdout] {line}");

                        if (line.Contains("IIS Express is running"))
                        {
                            wh.Set();
                        }
                    }
                },
                TaskCreationOptions.LongRunning);

            Task.Factory.StartNew(
                () =>
                {
                    string line;
                    while ((line = process.StandardError.ReadLine()) != null)
                    {
                        Output.WriteLine($"[webserver][stderr] {line}");
                    }
                },
                TaskCreationOptions.LongRunning);

            wh.WaitOne(5000);

            // Wait for iis express to finish starting up
            var retries = 5;
            while (true)
            {
                var usedPorts = IPGlobalProperties.GetIPGlobalProperties()
                                                  .GetActiveTcpListeners()
                                                  .Select(ipEndPoint => ipEndPoint.Port);

                if (usedPorts.Contains(iisPort))
                {
                    break;
                }

                retries--;

                if (retries == 0)
                {
                    throw new Exception("Gave up waiting for IIS Express.");
                }

                Thread.Sleep(1500);
            }

            return (process, newConfig);
        }

        public void EnableIast(bool enable = true)
        {
            SetEnvironmentVariable(ConfigurationKeys.Iast.Enabled, enable.ToString().ToLower());
        }

        public void EnableEvidenceRedaction(bool? enable = null)
        {
            if (enable != null)
            {
                SetEnvironmentVariable(ConfigurationKeys.Iast.RedactionEnabled, enable.ToString().ToLower());
            }
        }

        public void DisableObfuscationQueryString()
        {
            SetEnvironmentVariable(ConfigurationKeys.ObfuscationQueryStringRegex, string.Empty);
        }

        public void SetEnvironmentVariable(string key, string value)
        {
            EnvironmentHelper.CustomEnvironmentVariables[key] = value;
        }

        protected void ValidateSpans<T>(IEnumerable<MockSpan> spans, Func<MockSpan, T> mapper, IEnumerable<T> expected)
        {
            var spanLookup = new Dictionary<T, int>();
            foreach (var span in spans)
            {
                var key = mapper(span);
                if (spanLookup.ContainsKey(key))
                {
                    spanLookup[key]++;
                }
                else
                {
                    spanLookup[key] = 1;
                }
            }

            var missing = new List<T>();
            foreach (var e in expected)
            {
                var found = spanLookup.ContainsKey(e);
                if (found)
                {
                    if (--spanLookup[e] <= 0)
                    {
                        spanLookup.Remove(e);
                    }
                }
                else
                {
                    missing.Add(e);
                }
            }

            foreach (var e in missing)
            {
                Assert.True(false, $"no span found for `{e}`, remaining spans: `{string.Join(", ", spanLookup.Select(kvp => $"{kvp.Key}").ToArray())}`");
            }
        }

        /// <summary>
        /// NOTE: Only use this for local debugging, don't set permanently in tests
        /// We have a dedicated run that tests with debug mode enabled, so want to make
        /// sure that "normal" runs don't set this flag.
        /// </summary>
        [Obsolete("Setting this forces debug mode, whereas we want to automatically test in both modes")]
        protected void EnableDebugMode()
        {
            EnvironmentHelper.DebugModeEnabled = true;
        }

        protected void SetServiceName(string serviceName)
        {
            SetEnvironmentVariable(ConfigurationKeys.ServiceName, serviceName);
        }

        protected void SetServiceVersion(string serviceVersion)
        {
            SetEnvironmentVariable(ConfigurationKeys.ServiceVersion, serviceVersion);
        }

        protected void SetSecurity(bool security)
        {
            SecurityEnabled = security;
            SetEnvironmentVariable(Configuration.ConfigurationKeys.AppSec.Enabled, security ? "true" : "false");
        }

        protected void SetInstrumentationVerification()
        {
            bool verificationEnabled = ShouldUseInstrumentationVerification();

            if (verificationEnabled)
            {
                SetEnvironmentVariable(ConfigurationKeys.LogDirectory, EnvironmentHelper.LogDirectory);
            }
        }

        protected void VerifyInstrumentation(Process process)
        {
            if (!ShouldUseInstrumentationVerification())
            {
                return;
            }

            var logDirectory = EnvironmentHelper.LogDirectory;
            InstrumentationVerification.VerifyInstrumentation(process, logDirectory);
        }

        protected bool ShouldUseInstrumentationVerification()
        {
            if (!EnvironmentTools.IsWindows())
            {
                // Instrumentation Verification is currently only supported only on Windows
                return false;
            }

            // verify instrumentation adds a lot of time to tests so we only run it on azure and if it a scheduled build.
            // Return 'true' to verify instrumentation on local machine.
            // return true;
            return EnvironmentHelper.IsRunningInAzureDevOps() && EnvironmentHelper.IsScheduledBuild();
        }

        protected void EnableDirectLogSubmission(int intakePort, string integrationName, string host = "integration_tests")
        {
            SetEnvironmentVariable(ConfigurationKeys.DirectLogSubmission.Host, host);
            SetEnvironmentVariable(ConfigurationKeys.DirectLogSubmission.Url, $"http://127.0.0.1:{intakePort}");
            SetEnvironmentVariable(ConfigurationKeys.DirectLogSubmission.EnabledIntegrations, integrationName);
            SetEnvironmentVariable(ConfigurationKeys.ApiKey, "DUMMY_KEY_REQUIRED_FOR_DIRECT_SUBMISSION");
        }

        protected async Task<IImmutableList<MockSpan>> GetWebServerSpans(
            string path,
            MockTracerAgent agent,
            int httpPort,
            HttpStatusCode expectedHttpStatusCode,
            int expectedSpanCount = 2,
            bool filterServerSpans = true)
        {
            using var httpClient = new HttpClient();

            // disable tracing for this HttpClient request
            httpClient.DefaultRequestHeaders.Add(HttpHeaderNames.TracingEnabled, "false");
            httpClient.DefaultRequestHeaders.Add(HttpHeaderNames.UserAgent, "testhelper");
            var testStart = DateTime.UtcNow;
            var response = await httpClient.GetAsync($"http://localhost:{httpPort}" + path);
            var content = await response.Content.ReadAsStringAsync();
            Output.WriteLine($"[http] {response.StatusCode} {content}");
            Assert.Equal(expectedHttpStatusCode, response.StatusCode);

            if (filterServerSpans)
            {
                agent.SpanFilters.Add(IsServerSpan);
            }

            return agent.WaitForSpans(
                count: expectedSpanCount,
                minDateTime: testStart,
                returnAllOperations: true);
        }

        protected async Task AssertWebServerSpan(
            string path,
            MockTracerAgent agent,
            int httpPort,
            HttpStatusCode expectedHttpStatusCode,
            bool isError,
            string expectedAspNetErrorType,
            string expectedAspNetErrorMessage,
            string expectedErrorType,
            string expectedErrorMessage,
            string expectedSpanType,
            string expectedOperationName,
            string expectedAspNetResourceName,
            string expectedResourceName,
            string expectedServiceVersion,
            SerializableDictionary expectedTags = null)
        {
            IImmutableList<MockSpan> spans;

            using (var httpClient = new HttpClient())
            {
                // disable tracing for this HttpClient request
                httpClient.DefaultRequestHeaders.Add(HttpHeaderNames.TracingEnabled, "false");
                var testStart = DateTime.UtcNow;
                var response = await httpClient.GetAsync($"http://localhost:{httpPort}" + path);
                var content = await response.Content.ReadAsStringAsync();
                Output.WriteLine($"[http] {response.StatusCode} {content}");
                Assert.Equal(expectedHttpStatusCode, response.StatusCode);

                agent.SpanFilters.Add(IsServerSpan);

                spans = agent.WaitForSpans(
                    count: 2,
                    minDateTime: testStart,
                    returnAllOperations: true);

                Assert.True(spans.Count == 2, $"expected two span, saw {spans.Count}");
            }

            var aspnetSpan = spans.FirstOrDefault(s => s.Name == "aspnet.request");
            var innerSpan = spans.FirstOrDefault(s => s.Name == expectedOperationName);

            Assert.NotNull(aspnetSpan);
            Assert.Equal(expectedAspNetResourceName, aspnetSpan.Resource);

            Assert.NotNull(innerSpan);
            Assert.Equal(expectedResourceName, innerSpan.Resource);

            foreach (var span in spans)
            {
                // base properties
                Assert.Equal(expectedSpanType, span.Type);

                // errors
                Assert.Equal(isError, span.Error == 1);
                if (span == aspnetSpan)
                {
                    Assert.Equal(expectedAspNetErrorType, span.Tags.GetValueOrDefault(Tags.ErrorType));
                    Assert.Equal(expectedAspNetErrorMessage, span.Tags.GetValueOrDefault(Tags.ErrorMsg));
                }
                else if (span == innerSpan)
                {
                    Assert.Equal(expectedErrorType, span.Tags.GetValueOrDefault(Tags.ErrorType));
                    Assert.Equal(expectedErrorMessage, span.Tags.GetValueOrDefault(Tags.ErrorMsg));
                }

                // other tags
                Assert.Equal(SpanKinds.Server, span.Tags.GetValueOrDefault(Tags.SpanKind));
                Assert.Equal(expectedServiceVersion, span.Tags.GetValueOrDefault(Tags.Version));
            }

            if (expectedTags?.Values is not null)
            {
                foreach (var expectedTag in expectedTags)
                {
                    Assert.Equal(expectedTag.Value, innerSpan.Tags.GetValueOrDefault(expectedTag.Key));
                }
            }
        }

        protected async Task AssertAspNetSpanOnly(
            string path,
            MockTracerAgent agent,
            int httpPort,
            HttpStatusCode expectedHttpStatusCode,
            bool isError,
            string expectedErrorType,
            string expectedErrorMessage,
            string expectedSpanType,
            string expectedResourceName,
            string expectedServiceVersion)
        {
            IImmutableList<MockSpan> spans;

            using (var httpClient = new HttpClient())
            {
                // disable tracing for this HttpClient request
                httpClient.DefaultRequestHeaders.Add(HttpHeaderNames.TracingEnabled, "false");
                var testStart = DateTime.UtcNow;
                var response = await httpClient.GetAsync($"http://localhost:{httpPort}" + path);
                var content = await response.Content.ReadAsStringAsync();
                Output.WriteLine($"[http] {response.StatusCode} {content}");
                Assert.Equal(expectedHttpStatusCode, response.StatusCode);

                spans = agent.WaitForSpans(
                    count: 1,
                    minDateTime: testStart,
                    operationName: "aspnet.request",
                    returnAllOperations: true);

                Assert.True(spans.Count == 1, $"expected two span, saw {spans.Count}");
            }

            var span = spans[0];

            // base properties
            Assert.Equal(expectedResourceName, span.Resource);
            Assert.Equal(expectedSpanType, span.Type);

            // errors
            Assert.Equal(isError, span.Error == 1);
            Assert.Equal(expectedErrorType, span.Tags.GetValueOrDefault(Tags.ErrorType));
            Assert.Equal(expectedErrorMessage, span.Tags.GetValueOrDefault(Tags.ErrorMsg));

            // other tags
            Assert.Equal(SpanKinds.Server, span.Tags.GetValueOrDefault(Tags.SpanKind));
            Assert.Equal(expectedServiceVersion, span.Tags.GetValueOrDefault(Tags.Version));
        }

        protected async Task ReportRetry(ITestOutputHelper outputHelper, int attemptsRemaining, Type testType, Exception ex = null)
        {
            var type = outputHelper.GetType();
            var testMember = type.GetField("test", BindingFlags.Instance | BindingFlags.NonPublic);
            var test = (ITest)testMember?.GetValue(outputHelper);
            var testFullName = type.FullName + test?.TestCase.DisplayName;

            outputHelper.WriteLine($"Error executing test: {testFullName}. {attemptsRemaining} attempts remaining. {ex}");

            // In addition to logging, send a metric that will help us get more information through tags
            var srcBranch = Environment.GetEnvironmentVariable("DD_LOGGER_BUILD_SOURCEBRANCH");

            var tags = $$"""
                "os.platform:{{SanitizeTagValue(FrameworkDescription.Instance.OSPlatform)}}",
                "os.architecture:{{SanitizeTagValue(EnvironmentTools.GetPlatform())}}",
                "target.framework:{{SanitizeTagValue(EnvironmentHelper.GetTargetFramework())}}",
                "test.name:{{SanitizeTagValue(testFullName)}}",
                "git.branch:{{SanitizeTagValue(srcBranch)}}"
            """;

            var payload = $$"""
                {
                    "series": [{
                        "metric": "dd_trace_dotnet.ci.tests.retries",
                        "type": 1,
                        "points": [{
                            "timestamp": {{((DateTimeOffset)DateTime.UtcNow).ToUnixTimeSeconds()}},
                            "value": 1
                            }],
                        "tags": [
                            {{tags}}
                        ]
                    }]
                }
            """;

            var content = new StringContent(payload, Encoding.UTF8, "application/json");
            var response = await _client.PostAsync("https://api.datadoghq.com/api/v2/series", content);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (response.StatusCode != HttpStatusCode.Accepted)
            {
                outputHelper.WriteLine($"Failed to submit metric to monitor retry. Response was: Code: {response.StatusCode}. Response: {responseContent}. Payload sent was: \"{payload}\"");
            }
        }

        private string SanitizeTagValue(string tag)
        {
            tag.TryConvertToNormalizedTagName(true, out var normalizedTag);
            return normalizedTag;
        }

        private bool IsServerSpan(MockSpan span) =>
            span.Tags.GetValueOrDefault(Tags.SpanKind) == SpanKinds.Server;

        protected internal class TupleList<T1, T2> : List<Tuple<T1, T2>>
        {
            public void Add(T1 item, T2 item2)
            {
                Add(new Tuple<T1, T2>(item, item2));
            }
        }
    }
}
