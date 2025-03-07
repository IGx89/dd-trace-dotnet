// <copyright file="ConnectionMultiplexerExecuteAsyncImplIntegration.cs" company="Datadog">
// Unless explicitly stated otherwise all files in this repository are licensed under the Apache 2 License.
// This product includes software developed at Datadog (https://www.datadoghq.com/). Copyright 2017 Datadog, Inc.
// </copyright>

using System;
using System.ComponentModel;
using Datadog.Trace.ClrProfiler.CallTarget;
using Datadog.Trace.Configuration;

namespace Datadog.Trace.ClrProfiler.AutoInstrumentation.Redis.StackExchange
{
    /// <summary>
    /// StackExchange.Redis.ConnectionMultiplexer.ExecuteAsyncImpl calltarget instrumentation
    /// </summary>
    [InstrumentMethod(
        AssemblyName = "StackExchange.Redis",
        TypeName = "StackExchange.Redis.ConnectionMultiplexer",
        MethodName = "ExecuteAsyncImpl",
        ReturnTypeName = "System.Threading.Tasks.Task`1<T>",
        ParameterTypeNames = new[] { "StackExchange.Redis.Message", "StackExchange.Redis.ResultProcessor`1[!!0]", ClrNames.Object, "StackExchange.Redis.ServerEndPoint" },
        MinimumVersion = "1.0.0",
        MaximumVersion = "2.*.*",
        IntegrationName = StackExchangeRedisHelper.IntegrationName)]
    [InstrumentMethod(
        AssemblyName = "StackExchange.Redis.StrongName",
        TypeName = "StackExchange.Redis.ConnectionMultiplexer",
        MethodName = "ExecuteAsyncImpl",
        ReturnTypeName = "System.Threading.Tasks.Task`1<T>",
        ParameterTypeNames = new[] { "StackExchange.Redis.Message", "StackExchange.Redis.ResultProcessor`1[!!0]", ClrNames.Object, "StackExchange.Redis.ServerEndPoint" },
        MinimumVersion = "1.0.0",
        MaximumVersion = "2.*.*",
        IntegrationName = StackExchangeRedisHelper.IntegrationName)]
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public class ConnectionMultiplexerExecuteAsyncImplIntegration
    {
        /// <summary>
        /// OnMethodBegin callback
        /// </summary>
        /// <typeparam name="TTarget">Type of the target</typeparam>
        /// <typeparam name="TMessage">Type of the message</typeparam>
        /// <typeparam name="TProcessor">Type of the result processor</typeparam>
        /// <typeparam name="TServerEndPoint">Type of the server end point</typeparam>
        /// <param name="instance">Instance value, aka `this` of the instrumented method.</param>
        /// <param name="message">Message instance</param>
        /// <param name="resultProcessor">Result processor instance</param>
        /// <param name="state">State instance</param>
        /// <param name="serverEndPoint">Server endpoint instance</param>
        /// <returns>Calltarget state value</returns>
        internal static CallTargetState OnMethodBegin<TTarget, TMessage, TProcessor, TServerEndPoint>(TTarget instance, TMessage message, TProcessor resultProcessor, object state, TServerEndPoint serverEndPoint)
            where TTarget : IConnectionMultiplexer
            where TMessage : IMessageData
        {
            string rawCommand = message.CommandAndKey ?? "COMMAND";
            StackExchangeRedisHelper.HostAndPort hostAndPort = StackExchangeRedisHelper.GetHostAndPort(instance.Configuration);

            Scope scope = RedisHelper.CreateScope(Tracer.Instance, StackExchangeRedisHelper.IntegrationId, StackExchangeRedisHelper.IntegrationName, hostAndPort.Host, hostAndPort.Port, rawCommand, StackExchangeRedisHelper.GetDb(message.Db));
            if (scope is not null)
            {
                return new CallTargetState(scope);
            }

            return CallTargetState.GetDefault();
        }

        /// <summary>
        /// OnAsyncMethodEnd callback
        /// </summary>
        /// <typeparam name="TTarget">Type of the target</typeparam>
        /// <typeparam name="TResponse">Type of the response, in an async scenario will be T of Task of T</typeparam>
        /// <param name="instance">Instance value, aka `this` of the instrumented method.</param>
        /// <param name="response">Response instance</param>
        /// <param name="exception">Exception instance in case the original code threw an exception.</param>
        /// <param name="state">Calltarget state value</param>
        /// <returns>A response value, in an async scenario will be T of Task of T</returns>
        internal static TResponse OnAsyncMethodEnd<TTarget, TResponse>(TTarget instance, TResponse response, Exception exception, in CallTargetState state)
        {
            state.Scope.DisposeWithException(exception);
            return response;
        }
    }
}
