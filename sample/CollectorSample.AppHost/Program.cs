var builder = DistributedApplication.CreateBuilder(args);

builder.AddOpenTelemetryCollector("collector", "config.yaml")
    .WithAppForwarding();

var apiService = builder.AddProject<Projects.CollectorSample_ApiService>("apiservice");

builder.AddProject<Projects.CollectorSample_Web>("webfrontend")
    .WithReference(apiService);

builder.Build().Run();
