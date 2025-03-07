﻿// <copyright file="AgentConfiguration.cs" company="Datadog">
// Unless explicitly stated otherwise all files in this repository are licensed under the Apache 2 License.
// This product includes software developed at Datadog (https://www.datadoghq.com/). Copyright 2017 Datadog, Inc.
// </copyright>

#nullable enable

namespace Datadog.Trace.Agent.DiscoveryService;

internal record AgentConfiguration
{
    public AgentConfiguration(
        string? configurationEndpoint,
        string? debuggerEndpoint,
        string? agentVersion,
        string? statsEndpoint,
        string? dataStreamsMonitoringEndpoint,
        string? eventPlatformProxyEndpoint,
        bool clientDropP0)
    {
        ConfigurationEndpoint = configurationEndpoint;
        DebuggerEndpoint = debuggerEndpoint;
        AgentVersion = agentVersion;
        StatsEndpoint = statsEndpoint;
        DataStreamsMonitoringEndpoint = dataStreamsMonitoringEndpoint;
        EventPlatformProxyEndpoint = eventPlatformProxyEndpoint;
        ClientDropP0s = clientDropP0;
    }

    public string? ConfigurationEndpoint { get; }

    public string? DebuggerEndpoint { get; }

    public string? AgentVersion { get; }

    public string? StatsEndpoint { get; }

    public string? DataStreamsMonitoringEndpoint { get; }

    public string? EventPlatformProxyEndpoint { get; }

    public bool ClientDropP0s { get; }
}
