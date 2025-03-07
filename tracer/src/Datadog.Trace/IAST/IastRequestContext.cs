// <copyright file="IastRequestContext.cs" company="Datadog">
// Unless explicitly stated otherwise all files in this repository are licensed under the Apache 2 License.
// This product includes software developed at Datadog (https://www.datadoghq.com/). Copyright 2017 Datadog, Inc.
// </copyright>

#nullable enable

using System.Collections.Generic;
using System.Xml.Linq;
using Datadog.Trace.AppSec;
using Datadog.Trace.Logging;

namespace Datadog.Trace.Iast;

internal class IastRequestContext
{
    private static readonly IDatadogLogger Log = DatadogLogging.GetLoggerFor(typeof(IastRequestContext));
    private VulnerabilityBatch? _vulnerabilityBatch;
    private object _vulnerabilityLock = new();
    private TaintedObjects _taintedObjects = new();
    private bool _routedParametersAdded = false;
    private bool _querySourcesAdded = false;

    internal void AddIastVulnerabilitiesToSpan(Span span)
    {
        span.Tags.SetTag(Tags.IastEnabled, "1");

        if (_vulnerabilityBatch != null)
        {
            span.Tags.SetTag(Tags.IastJson, _vulnerabilityBatch.ToString());
        }
    }

    internal bool AddVulnerabilitiesAllowed()
    {
        return ((_vulnerabilityBatch?.Vulnerabilities.Count ?? 0) < Iast.Instance.Settings.VulnerabilitiesPerRequest);
    }

    internal void AddVulnerability(Vulnerability vulnerability)
    {
        lock (_vulnerabilityLock)
        {
            _vulnerabilityBatch ??= IastModule.GetVulnerabilityBatch();
            _vulnerabilityBatch.Add(vulnerability);
        }
    }

    internal void AddRequestBody(object body, object? bodyExtracted)
    {
        try
        {
            if (bodyExtracted is null)
            {
                bodyExtracted = ObjectExtractor.Extract(body);
            }

            AddExtractedBody(bodyExtracted, null);
        }
        catch
        {
            Log.Warning("Error reading request Body.");
        }
    }

    private void AddExtractedBody(object bodyExtracted, string? key)
    {
        if (bodyExtracted != null)
        {
            // We get either string, List<object> or Dictionary<string, object>
            if (bodyExtracted is string bodyExtractedStr)
            {
                _taintedObjects.TaintInputString(bodyExtractedStr, new Source(SourceType.GetByte(SourceTypeName.RequestBody), key, bodyExtractedStr));
            }
            else
            {
                if (bodyExtracted is List<object> bodyExtractedList)
                {
                    foreach (var element in bodyExtractedList)
                    {
                        AddExtractedBody(element, key);
                    }
                }
                else
                {
                    if (bodyExtracted is Dictionary<string, object> bodyExtractedDic)
                    {
                        foreach (var keyValue in bodyExtractedDic)
                        {
                            AddExtractedBody(keyValue.Value, keyValue.Key);
                            _taintedObjects.TaintInputString(keyValue.Key, new Source(SourceType.GetByte(SourceTypeName.RequestBody), key, keyValue.Key));
                        }
                    }
                }
            }
        }
    }

    private void AddRequestParameter(string name, string value)
    {
        _taintedObjects.TaintInputString(value, new Source(SourceType.GetByte(SourceTypeName.RequestParameterValue), name, value));
        _taintedObjects.TaintInputString(name, new Source(SourceType.GetByte(SourceTypeName.RequestParameterName), name, null));
    }

    private void AddRoutedParameter(string name, string value)
    {
        _taintedObjects.TaintInputString(value, new Source(SourceType.GetByte(SourceTypeName.RoutedParameterValue), name, value));
    }

    private void AddQueryStringRaw(string queryString)
    {
        _taintedObjects.TaintInputString(queryString, new Source(SourceType.GetByte(SourceTypeName.RequestQuery), null, queryString));
    }

    private void AddQueryPath(string path)
    {
        _taintedObjects.TaintInputString(path, new Source(SourceType.GetByte(SourceTypeName.RequestPath), null, path));
    }

    private void AddRouteData(IDictionary<string, object> routeData)
    {
        if (!_routedParametersAdded)
        {
            if (routeData != null)
            {
                foreach (var item in routeData)
                {
                    if (item.Value is string valueAsString)
                    {
                        AddRoutedParameter(item.Key, valueAsString);
                    }
                }
            }

            _routedParametersAdded = true;
        }
    }

    internal TaintedObjects GetTaintedObjects()
    {
        return _taintedObjects;
    }

    internal TaintedObject? GetTainted(object objectToFind)
    {
        return _taintedObjects.Get(objectToFind);
    }

#if NETFRAMEWORK
    // It might happen that we call more than once this method depending on the asp version. Anyway, these calls would be sequential.
    internal void AddRequestData(System.Web.HttpRequest request)
    {
        if (!_querySourcesAdded)
        {
            if (request.QueryString != null)
            {
                foreach (var key in request.QueryString.AllKeys)
                {
                    AddRequestParameter(key, request.QueryString[key]);
                }

                AddQueryStringRaw(request.QueryString.ToString());
            }

            AddRequestHeaders(request.Headers);
            AddQueryPath(request.Path);
            _querySourcesAdded = true;
        }
    }

    private void AddRequestHeaders(System.Collections.Specialized.NameValueCollection? headers)
    {
        if (headers is not null)
        {
            foreach (var header in headers.AllKeys)
            {
                AddHeaderData(header, headers[header]);
            }
        }
    }

    // It might happen that we call more than once this method depending on the asp version. Anyway, these calls would be sequential.
    internal void AddRequestData(System.Web.HttpRequest request, IDictionary<string, object> routeData)
    {
        AddRouteData(routeData);
        AddRequestData(request);
    }

#else
    // It might happen that we call more than once this method depending on the asp version. Anyway, these calls would be sequential.
    internal void AddRequestData(Microsoft.AspNetCore.Http.HttpRequest request, IDictionary<string, object> routeParameters)
    {
        AddRouteData(routeParameters);

        if (!_querySourcesAdded)
        {
            if (request.Query != null)
            {
                foreach (var item in request.Query)
                {
                    AddRequestParameter(item.Key, item.Value);
                }
            }

            AddQueryPath(request.Path);
            AddQueryStringRaw(request.QueryString.Value);
            AddRequestHeaders(request.Headers);
            _querySourcesAdded = true;
        }
    }

    private void AddRequestHeaders(Microsoft.AspNetCore.Http.IHeaderDictionary? headers)
    {
        if (headers is not null)
        {
            foreach (var header in headers)
            {
                if (header.Value.Count > 1)
                {
                    foreach (var singleValue in header.Value)
                    {
                        AddHeaderData(header.Key, singleValue);
                    }
                }
                else
                {
                    AddHeaderData(header.Key, header.Value);
                }
            }
        }
    }

#endif

    private void AddHeaderData(string name, string value)
    {
        _taintedObjects.TaintInputString(value, new Source(SourceType.GetByte(SourceTypeName.RequestHeader), name, value));
        _taintedObjects.TaintInputString(name, new Source(SourceType.GetByte(SourceTypeName.RequestHeaderName), name, null));
    }
}
