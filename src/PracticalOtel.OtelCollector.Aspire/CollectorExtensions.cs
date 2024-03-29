using Aspire.Hosting.Lifecycle;
using Aspire.Hosting.Utils;
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

        var dashboardOtlpEndpoint = HostNameResolver.ReplaceLocalhostWithContainerHost(url, builder.Configuration);

        var resource = new CollectorResource(name);
        return builder.AddResource(resource)
            .WithEndpoint(hostPort: 4317, name: CollectorResource.GRPCEndpointName, scheme: "http")
            .WithEndpoint(hostPort: 4318, name: CollectorResource.HTTPEndpointName, scheme: "http")
            .WithAnnotation(new ContainerImageAnnotation { Image = "otel/opentelemetry-collector-contrib", Tag = "latest" })
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

}