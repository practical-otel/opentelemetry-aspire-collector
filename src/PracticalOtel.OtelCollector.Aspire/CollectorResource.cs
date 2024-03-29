using Aspire.Hosting.ApplicationModel;

namespace PracticalOtel.OtelCollector.Aspire;

public class CollectorResource : ContainerResource, IResourceWithEndpoints
{
    internal static string GRPCEndpointName = "grpc";
    internal static string HTTPEndpointName = "http";

    public EndpointReference GRPCEndpoint { get; }
    public EndpointReference HTTPEndpoint { get; }

    public CollectorResource(string name) : base(name)
    {
        GRPCEndpoint = new(this, GRPCEndpointName);
        HTTPEndpoint = new(this, HTTPEndpointName);
    }
}
