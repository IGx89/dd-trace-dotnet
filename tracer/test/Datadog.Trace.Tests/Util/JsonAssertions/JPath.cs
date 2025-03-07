﻿// <copyright file="JPath.cs" company="Datadog">
// Unless explicitly stated otherwise all files in this repository are licensed under the Apache 2 License.
// This product includes software developed at Datadog (https://www.datadoghq.com/). Copyright 2017 Datadog, Inc.
// </copyright>

/// Based on https://github.com/fluentassertions/fluentassertions.json

using System.Collections.Generic;

namespace Datadog.Trace.Tests.Util.JsonAssertions;

internal class JPath
{
    private readonly List<string> nodes = new();

    public JPath()
    {
        nodes.Add("$");
    }

    private JPath(JPath existingPath, string extraNode)
    {
        nodes.AddRange(existingPath.nodes);
        nodes.Add(extraNode);
    }

    public JPath AddProperty(string name)
    {
        return new JPath(this, $".{name}");
    }

    public JPath AddIndex(int index)
    {
        return new JPath(this, $"[{index}]");
    }

    public override string ToString()
    {
        return string.Concat(nodes);
    }
}
