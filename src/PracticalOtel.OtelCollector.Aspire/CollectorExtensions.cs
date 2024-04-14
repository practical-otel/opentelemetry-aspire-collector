using Aspire.Hosting.ApplicationModel;
using Aspire.Hosting.Lifecycle;
using Microsoft.Extensions.Configuration;
using PracticalOtel.OtelCollector.Aspire;

namespace Aspire.Hosting;

public static class CollectorExtensions
{
    private const string DashboardOtlpUrlVariableName = "DOTNET_DASHBOARD_OTLP_ENDPOINT_URL";
    private const string DashboardOtlpUrlDefaultValue = "http://localhost:18889";

    /// <summary>
    /// Adds an OpenTelemetry Collector into the Aspire AppHost
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="name"></param>
    /// <param name="configFileLocation"></param>
    /// <returns></returns>
    public static IResourceBuilder<CollectorResource> AddOpenTelemetryCollector(this IDistributedApplicationBuilder builder, string name, string configFileLocation)
    {
        var url = builder.Configuration[DashboardOtlpUrlVariableName] ?? DashboardOtlpUrlDefaultValue;

        var dashboardOtlpEndpoint = ReplaceLocalhostWithContainerHost(url, builder.Configuration);

        var resource = new CollectorResource(name);
        return builder.AddResource(resource)
            .WithImage("ghcr.io/open-telemetry/opentelemetry-collector-releases/opentelemetry-collector-contrib", "latest")
            .WithEndpoint(port: 4317, targetPort:4317, name: CollectorResource.GRPCEndpointName, scheme: "http")
            .WithEndpoint(port: 4318, targetPort:4318, name: CollectorResource.HTTPEndpointName, scheme: "http")
            .WithBindMount(configFileLocation, "/etc/otelcol-contrib/config.yaml")
            .WithEnvironment("ASPIRE_ENDPOINT", dashboardOtlpEndpoint);
    }

    /// <summary>
    /// Force all apps to forward to the collector instead of the dashboard directly
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static IResourceBuilder<CollectorResource> WithAppForwarding(this IResourceBuilder<CollectorResource> builder)
    {
        builder.ApplicationBuilder.Services.TryAddLifecycleHook<EnvironmentVariableHook>();
        return builder;
    }

    private static string ReplaceLocalhostWithContainerHost(string value, IConfiguration configuration)
    {
        var hostName = configuration["AppHost:ContainerHostname"] ?? "host.docker.internal";

        return value.Replace("localhost", hostName, StringComparison.OrdinalIgnoreCase)
                    .Replace("127.0.0.1", hostName)
                    .Replace("[::1]", hostName);
    }
}