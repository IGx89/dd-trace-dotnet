﻿// <copyright file="RuleSet.cs" company="Datadog">
// Unless explicitly stated otherwise all files in this repository are licensed under the Apache 2 License.
// This product includes software developed at Datadog (https://www.datadoghq.com/). Copyright 2017 Datadog, Inc.
// </copyright>

#nullable enable
using System.Collections.Generic;
using Datadog.Trace.Vendors.Newtonsoft.Json;
using Datadog.Trace.Vendors.Newtonsoft.Json.Linq;

namespace Datadog.Trace.AppSec.Rcm.Models.AsmDd;

internal class RuleSet
{
    internal string? Version { get; set; }

    internal JToken? Metadata { get; set; }

    [JsonProperty("rules")]
    internal JToken? Rules { get; set; }

    public JToken? All { get; set; }

    public static RuleSet From(JToken result)
    {
        var ruleset = new RuleSet { Version = result["version"]?.ToString(), Metadata = result["metadata"], Rules = result["rules"], All = result };
        return ruleset;
    }

    /// <summary>
    /// on the contrary to other data, this one should be added at the root because it's shaped as {"version": "2.2","metadata": {"rules_version": "1.3.2"},"rules": []}
    /// </summary>
    /// <param name="dictionary">dictionary</param>
    public void AddToDictionaryAtRoot(Dictionary<string, object> dictionary)
    {
        if (Rules != null)
        {
            dictionary.Add("rules", Rules);
        }

        if (Metadata != null)
        {
            dictionary.Add("metadata", Metadata);
        }

        if (Version != null)
        {
            dictionary.Add("version", Version);
        }
    }
}
