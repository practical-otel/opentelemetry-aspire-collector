# Aspire Hosting extension for the OpenTelemetry Collector

This is an extension that enables you to use the OpenTelemetry Collector inside your Aspire application.

It's considered best practice to use an OpenTelemetry Collector in production to perform telemetry sanitisation and curation using the Filter, Transform and Redaction processors amongst other uses. Since Aspire is replicating an environment like production, including telemetry, this provides a convenient way for you to test to ensure that your telemetry is able to be processed correctly.

## Usage

In your AppHost project

```csharp

var builder = DistributedApplication.CreateBuilder(args);

builder.AddOpenTelemetryCollector("collector", "config.yaml")
    .WithAppForwarding();

// your application setup

builder.Build().Run();
```

Here's an example OpenTelemetry Config

```yaml
receivers:
  otlp:
    protocols:
      grpc:
      http:

processors:
  batch:

exporters:
  debug:
    verbosity: detailed
  otlp/aspire:
    endpoint: $ASPIRE_ENDPOINT
    tls:
        insecure: true
  
service:
  pipelines:
    traces:
      receivers: [otlp]
      processors: [batch]
      exporters: [otlp/aspire]
    metrics:
      receivers: [otlp]
      processors: [batch]
      exporters: [otlp/aspire]
    logs:
      receivers: [otlp]
      processors: [batch]
      exporters: [otlp/aspire]
```

This is the most basic config, and doesn't provide any additional capabilities.

```yaml
receivers:
  otlp:
    protocols:
      grpc:
      http:

processors:
  batch:
  filter/healthcheck:
    traces:
        span:
          - 'attributes["http.target"] == "/healthcheck"'
  redaction/creditcard:
    allow_all_keys: true
    blocked_values:
      - "4[0-9]{12}(?:[0-9]{3})?" ## Visa credit card number
      - "(5[1-5][0-9]{14})"       ## MasterCard number
  transform/environment:
    error_mode: ignore
    trace_statements:
      - context: resource
        statements:
          - set(attributes["environment"], "development")
    log_statements:
      - context: resource
        statements:
          - set(attributes["environment"], "development")
    metric_statements:
      - context: resource
        statements:
          - set(attributes["environment"], "development")
    

exporters:
  otlp/aspire:
    endpoint: $ASPIRE_ENDPOINT
    tls:
        insecure: true
  
service:
  pipelines:
    traces:
      receivers: [otlp]
      processors: 
        - batch
        - filter/healthcheck
        - redaction/creditcard
        - transform/environment
      exporters: [otlp/aspire]
    metrics:
      receivers: [otlp]
      processors: 
        - batch
        - transform/environment
      exporters: [otlp/aspire]
    logs:
      receivers: [otlp]
      processors: 
        - batch
        - transform/environment
      exporters: [otlp/aspire]
```

This config shows the usage of 3 separate processors:

* Filter Processor to remove spans from the healthcheck
* Redaction Processor to replace creditcard numbers with stars
* Transform Processor to add additional attributes to the signal's resource attributes.

***Note:** Aspire currently doesn't support Resource attributes for Logs and Metrics*

## Limitations

This is currently a work in progress, I plan to add the ability to selectively add applications to the pipeline, but as of right now, you need to add `.WithAppForwarding()` and this will do all the apps.

This currently has to use a config file in your solution, ultimately, I'm planning on moving this to a Code generated configuration.
