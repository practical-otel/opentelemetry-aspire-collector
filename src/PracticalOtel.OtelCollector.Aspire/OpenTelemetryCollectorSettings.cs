namespace Aspire.Hosting;

/// <summary>
/// Settings for the OpenTelemetry Collector
/// </summary>
public class OpenTelemetryCollectorSettings
{
    /// <summary>
    /// The version of the collector, defaults to latest
    /// </summary>
    public string CollectorVersion { get; set; } = "latest";

    /// <summary>
    /// The image of the collector, defaults to ghcr.io/open-telemetry/opentelemetry-collector-releases/opentelemetry-collector-contrib
    /// </summary>
    public string CollectorImage { get; set; } = "ghcr.io/open-telemetry/opentelemetry-collector-releases/opentelemetry-collector-contrib";

    public bool ForceNonSecureReceiver { get; set; } = false;

    public bool EnableGrpcEndpoint { get; set; } = true;
    public bool EnableHttpEndpoint { get; set; } = true;
}
