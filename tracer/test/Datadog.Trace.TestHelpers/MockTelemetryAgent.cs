﻿// <copyright file="MockTelemetryAgent.cs" company="Datadog">
// Unless explicitly stated otherwise all files in this repository are licensed under the Apache 2 License.
// This product includes software developed at Datadog (https://www.datadoghq.com/). Copyright 2017 Datadog, Inc.
// </copyright>

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Datadog.Trace.Telemetry;
using Datadog.Trace.Telemetry.Transports;
using Datadog.Trace.Vendors.Newtonsoft.Json;
using Datadog.Trace.Vendors.Newtonsoft.Json.Serialization;

namespace Datadog.Trace.TestHelpers
{
    internal class MockTelemetryAgent : IDisposable
    {
        private readonly HttpListener _listener;
        private readonly Thread _listenerThread;

        public MockTelemetryAgent(int port = 8524, int retries = 5)
        {
            // try up to 5 consecutive ports before giving up
            while (true)
            {
                // seems like we can't reuse a listener if it fails to start,
                // so create a new listener each time we retry
                var listener = new HttpListener();
                listener.Prefixes.Add($"http://127.0.0.1:{port}/");
                listener.Prefixes.Add($"http://localhost:{port}/");

                try
                {
                    listener.Start();

                    // successfully listening
                    Port = port;
                    _listener = listener;

                    _listenerThread = new Thread(HandleHttpRequests);
                    _listenerThread.Start();

                    return;
                }
                catch (HttpListenerException) when (retries > 0)
                {
                    // only catch the exception if there are retries left
                    port = TcpPortProvider.GetOpenPort();
                    retries--;
                }
                catch
                {
                    // always close listener if exception is thrown,
                    // whether it was caught or not
                    listener.Close();
                    throw;
                }

                // always close listener if exception is thrown,
                // whether it was caught or not
                listener.Close();
            }
        }

        public event EventHandler<EventArgs<HttpListenerContext>> RequestReceived;

        /// <summary>
        /// Gets or sets a value indicating whether to skip serialization of traces.
        /// </summary>
        public bool ShouldDeserializeTraces { get; set; } = true;

        /// <summary>
        /// Gets the TCP port that this Agent is listening on.
        /// Can be different from <see cref="MockTelemetryAgent"/>'s <c>initialPort</c>
        /// parameter if listening on that port fails.
        /// </summary>
        public int Port { get; }

        public ConcurrentStack<TelemetryData> Telemetry { get; } = new();

        public IImmutableList<NameValueCollection> RequestHeaders { get; private set; } = ImmutableList<NameValueCollection>.Empty;

        /// <summary>
        /// Wait for the telemetry condition to be satisfied.
        /// Note that the first telemetry that satisfies the condition is returned
        /// To retrieve all telemetry received, use <see cref="Telemetry"/>
        /// </summary>
        /// <param name="hasExpectedValues">A predicate for the current telemetry</param>
        /// <param name="timeoutInMilliseconds">The timeout</param>
        /// <param name="sleepTime">The time between checks</param>
        /// <returns>The telemetry that satisfied <paramref name="hasExpectedValues"/></returns>
        public TelemetryData WaitForLatestTelemetry(
            Func<TelemetryData, bool> hasExpectedValues,
            int timeoutInMilliseconds = 5000,
            int sleepTime = 200)
        {
            var deadline = DateTime.UtcNow.AddMilliseconds(timeoutInMilliseconds);

            TelemetryData latest = default;
            while (DateTime.UtcNow < deadline)
            {
                if (Telemetry.TryPeek(out latest) && hasExpectedValues(latest))
                {
                    break;
                }

                Thread.Sleep(sleepTime);
            }

            return latest;
        }

        public void Dispose()
        {
            _listener?.Close();
        }

        internal static TelemetryData DeserializeResponse(Stream inputStream, string apiVersion, string requestType)
        {
            var serializer = TelemetryConverter.GetSerializer(apiVersion, requestType);
            TelemetryData telemetry;
            using var sr = new StreamReader(inputStream);
            var text = sr.ReadToEnd();
            var tr = new StringReader(text);
            using (var jsonTextReader = new JsonTextReader(tr))
            {
                telemetry = serializer.Deserialize<TelemetryData>(jsonTextReader);
            }

            return telemetry;
        }

        protected virtual void OnRequestReceived(HttpListenerContext context)
        {
            RequestReceived?.Invoke(this, new EventArgs<HttpListenerContext>(context));
        }

        protected virtual void HandleHttpRequest(HttpListenerContext ctx)
        {
            OnRequestReceived(ctx);

            var apiVersion = ctx.Request.Headers[TelemetryConstants.ApiVersionHeader];
            var requestType = ctx.Request.Headers[TelemetryConstants.RequestTypeHeader];

            var telemetry = DeserializeResponse(ctx.Request.InputStream, apiVersion, requestType);
            Telemetry.Push(telemetry);

            lock (this)
            {
                RequestHeaders = RequestHeaders.Add(new NameValueCollection(ctx.Request.Headers));
            }

            // NOTE: HttpStreamRequest doesn't support Transfer-Encoding: Chunked
            // (Setting content-length avoids that)

            ctx.Response.StatusCode = 200;
            ctx.Response.Close();
        }

        private void HandleHttpRequests()
        {
            while (_listener.IsListening)
            {
                try
                {
                    var ctx = _listener.GetContext();
                    HandleHttpRequest(ctx);
                }
                catch (HttpListenerException)
                {
                    // listener was stopped,
                    // ignore to let the loop end and the method return
                }
                catch (ObjectDisposedException)
                {
                    // the response has been already disposed.
                }
                catch (InvalidOperationException)
                {
                    // this can occur when setting Response.ContentLength64, with the framework claiming that the response has already been submitted
                    // for now ignore, and we'll see if this introduces downstream issues
                }
                catch (Exception) when (!_listener.IsListening)
                {
                    // we don't care about any exception when listener is stopped
                }
            }
        }

        internal class TelemetryConverter
        {
            private static readonly Dictionary<string, JsonSerializer> V1Serializers;

            static TelemetryConverter()
            {
                V1Serializers = new()
                {
                    { TelemetryRequestTypes.AppStarted, CreateSerializer<AppStartedPayload>() },
                    { TelemetryRequestTypes.AppDependenciesLoaded, CreateSerializer<AppDependenciesLoadedPayload>() },
                    { TelemetryRequestTypes.AppIntegrationsChanged, CreateSerializer<AppIntegrationsChangedPayload>() },
                    { TelemetryRequestTypes.GenerateMetrics, CreateSerializer<GenerateMetricsPayload>() },
                    { TelemetryRequestTypes.AppClosing, CreateNullPayloadSerializer() },
                    { TelemetryRequestTypes.AppHeartbeat, CreateNullPayloadSerializer() },
                };
            }

            public static JsonSerializer GetSerializer(string apiVersion, string requestType)
            {
                if (apiVersion == TelemetryConstants.ApiVersion)
                {
                    if (V1Serializers.TryGetValue(requestType, out var serializer))
                    {
                        return serializer;
                    }
                }

                throw new Exception($"Unknown {apiVersion} telemetry request type {requestType} ");
            }

            // Needs to be kept in sync with JsonTelemetryTransport.SerializerSettings, but with the additional converter
            private static JsonSerializer CreateSerializer<TPayload>() =>
                JsonSerializer.Create(new JsonSerializerSettings
                {
                    NullValueHandling = JsonTelemetryTransport.SerializerSettings.NullValueHandling,
                    ContractResolver = JsonTelemetryTransport.SerializerSettings.ContractResolver,
                    Converters = new List<JsonConverter> { new PayloadConverter<TPayload>(), new IntegrationTelemetryDataConverter() },
                });

            private static JsonSerializer CreateNullPayloadSerializer() =>
                JsonSerializer.Create(new JsonSerializerSettings
                {
                    NullValueHandling = JsonTelemetryTransport.SerializerSettings.NullValueHandling,
                    ContractResolver = JsonTelemetryTransport.SerializerSettings.ContractResolver,
                });
        }

        private class PayloadConverter<TPayload> : JsonConverter
        {
            public override bool CanConvert(Type objectType)
                => objectType == typeof(IPayload);

            // use the default serialization - it works fine
            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
                => serializer.Serialize(writer, value);

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
                => serializer.Deserialize<TPayload>(reader);
        }

        private class IntegrationTelemetryDataConverter : JsonConverter
        {
            public override bool CanConvert(Type objectType)
            {
                return objectType == typeof(IntegrationTelemetryData);
            }

            // use the default serialization - it works fine
            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
                => serializer.Serialize(writer, value);

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                // This is a pain, but for some reason Json.NET refuses to deserialize it otherwise
                string name = null;
                bool? enabled = null;
                bool? autoEnabled = null;
                string error = null;

                var contractResolver = (DefaultContractResolver)serializer.ContractResolver;
                var nameProperty = contractResolver.GetResolvedPropertyName(nameof(IntegrationTelemetryData.Name));
                var errorProperty = contractResolver.GetResolvedPropertyName(nameof(IntegrationTelemetryData.Error));
                var enabledProperty = contractResolver.GetResolvedPropertyName(nameof(IntegrationTelemetryData.Enabled));
                var autoEnabledProperty = contractResolver.GetResolvedPropertyName(nameof(IntegrationTelemetryData.AutoEnabled));

                while (reader.Read())
                {
                    if (reader.TokenType != JsonToken.PropertyName)
                    {
                        break;
                    }

                    var propertyName = (string)reader.Value;
                    if (!reader.Read())
                    {
                        continue;
                    }

                    if (nameProperty.Equals(propertyName, StringComparison.OrdinalIgnoreCase))
                    {
                        name = serializer.Deserialize<string>(reader);
                        continue;
                    }

                    if (errorProperty.Equals(propertyName, StringComparison.OrdinalIgnoreCase))
                    {
                        error = serializer.Deserialize<string>(reader);
                        continue;
                    }

                    if (enabledProperty.Equals(propertyName, StringComparison.OrdinalIgnoreCase))
                    {
                        enabled = serializer.Deserialize<bool>(reader);
                        continue;
                    }

                    if (autoEnabledProperty.Equals(propertyName, StringComparison.OrdinalIgnoreCase))
                    {
                        autoEnabled = serializer.Deserialize<bool>(reader);
                    }
                }

                if (name is null || enabled is null)
                {
                    throw new InvalidDataException($"Missing properties {nameProperty} and {enabledProperty} in serialized {nameof(IntegrationTelemetryData)}");
                }

                return new IntegrationTelemetryData(name, enabled.Value, autoEnabled, error);
            }
        }
    }
}
