﻿using System.Diagnostics;
using OpenTelemetry;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
// ReSharper disable MoveLocalFunctionAfterJumpStatement
// ReSharper disable once AccessToDisposedClosure

const string appName = "ConsoleApp"; 
var activitySource = new ActivitySource(appName);
var resourceBuilder = ResourceBuilder.CreateDefault().AddService(serviceName: appName);
using var traceProvider = Sdk.CreateTracerProviderBuilder()
    .SetResourceBuilder(resourceBuilder)
    .AddSource(activitySource.Name)
    .AddOtlpExporter()
    .AddConsoleExporter()
    .AddHttpClientInstrumentation()
    .Build();

#region methods
// root span
void RunMainJob()
{
    using var activity = activitySource.StartActivity(name: nameof(RunMainJob));
    RunShortTimeJob();
    RunLongTimeJob();
}

// child span1
void RunShortTimeJob()
{
    using var activity = activitySource.StartActivity(name: nameof(RunShortTimeJob));

    Thread.Sleep(1000);
}

// child span2
void RunLongTimeJob()
{
    using var activity = activitySource.StartActivity(name: nameof(RunLongTimeJob));
    
    Thread.Sleep(2000);
    using var httpClient = new HttpClient();
    var result = httpClient.GetStringAsync("https://catfact.ninja/fact").GetAwaiter().GetResult();
    activity?.SetTag("result", result);
}

#endregion

RunMainJob();