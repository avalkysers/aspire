using Aspire.WeatherForecast.ServiceDefaults;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using Polly.Simmy;

namespace Microsoft.Extensions.Hosting;

// Adds common .NET Aspire services: service discovery, resilience, health checks, and OpenTelemetry.
// This project should be referenced by each service project in your solution.
// To learn more about using this project, see https://aka.ms/dotnet/aspire/service-defaults
public static class Extensions
{
    public static bool ShouldSkipAspireServiceDiscovery(this IHostApplicationBuilder builder)
    {
        return builder.Configuration["ASPIRE:SKIP_SERVICE_DISCOVERY"] == "true";
    }

    private static bool ShouldSkipAspireHealthCheck(this IHostApplicationBuilder builder)
    {
        return builder.Configuration["ASPIRE:SKIP_HEALTH_CHECK"] == "true";
    }

    public static IHostApplicationBuilder AddServiceDefaults(this IHostApplicationBuilder builder)
    {
        // Load global settings from apphost for show-casing...
        builder.Configuration.AddJsonFile(Path.Combine(AppContext.BaseDirectory, "aspirehosting.appsettings.json"), optional: false);
        
        builder.ConfigureOpenTelemetry();

        if(!builder.ShouldSkipAspireHealthCheck())
        {
            builder.AddDefaultHealthChecks();
        }

        if(!builder.ShouldSkipAspireServiceDiscovery())
        {
            builder.Services.AddServiceDiscovery();
        }

        builder.Services.ConfigureHttpClientDefaults(http =>
        {
            // Turn on resilience by default
            http.AddStandardResilienceHandler();

            if(!builder.ShouldSkipAspireServiceDiscovery())
            {
                // Turn on service discovery by default
                http.AddServiceDiscovery();
            }
        });

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        return builder;
    }

    private static IHostApplicationBuilder ConfigureOpenTelemetry(this IHostApplicationBuilder builder)
    {
        builder.Logging.AddOpenTelemetry(logging =>
        {
            logging.IncludeFormattedMessage = true;
            logging.IncludeScopes = true;
        });

        builder.Services.AddOpenTelemetry()
            .WithMetrics(metrics =>
            {
                metrics.AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddRuntimeInstrumentation();
            })
            .WithTracing(tracing =>
            {
                tracing.AddAspNetCoreInstrumentation()
                    // Uncomment the following line to enable gRPC instrumentation (requires the OpenTelemetry.Instrumentation.GrpcNetClient package)
                    //.AddGrpcClientInstrumentation()
                    .AddHttpClientInstrumentation();
            });

        builder.AddOpenTelemetryExporters();

        return builder;
    }

    private static IHostApplicationBuilder AddOpenTelemetryExporters(this IHostApplicationBuilder builder)
    {
        var useOtlpExporter = !string.IsNullOrWhiteSpace(builder.Configuration["OTEL_EXPORTER_OTLP_ENDPOINT"]);

        if (useOtlpExporter)
        {
            builder.Services.AddOpenTelemetry().UseOtlpExporter();
        }

        // Uncomment the following lines to enable the Azure Monitor exporter (requires the Azure.Monitor.OpenTelemetry.AspNetCore package)
        //if (!string.IsNullOrEmpty(builder.Configuration["APPLICATIONINSIGHTS_CONNECTION_STRING"]))
        //{
        //    builder.Services.AddOpenTelemetry()
        //       .UseAzureMonitor();
        //}

        return builder;
    }

    private static IHostApplicationBuilder AddDefaultHealthChecks(this IHostApplicationBuilder builder)
    {
        builder.Services.AddHealthChecks()
            // Add a default liveness check to ensure app is responsive
            .AddCheck("self", () => HealthCheckResult.Healthy(), ["live"]);

        return builder;
    }

    public static Chaos LookForChaosIfEnabled(this WebApplication app) 
    {
        var chaos = ChaosMonkey.NoChaos();

        if(app.Configuration["CHAOS_MONKEY_ENABLED"] == "true")
        {
            try
            {
                chaos = ChaosMonkey.Create();
            }
            catch(ChaosMonkeyException ex)
            {
                app.Logger.LogWarning("Chaos detected: An exception was thrown ({message}).", ex.Message);
                throw;
            }
        }

        if(chaos.Created)
        {
            app.Logger.LogWarning("Chaos detected: Execution was delayed for {delay} seconds.", chaos.DelayInSeconds);
        }

        return chaos;
    }

    private static bool ShouldSkipHealthCheck(this WebApplication app)
    {
        return app.Configuration["ASPIRE:SKIP_HEALTH_CHECK"] == "true";
    }

    public static WebApplication MapDefaultEndpoints(this WebApplication app)
    {
        // Adding health checks endpoints to applications in non-development environments has security implications.
        // See https://aka.ms/dotnet/aspire/healthchecks for details before enabling these endpoints in non-development environments.
        if (app.Environment.IsDevelopment())
        {
            if(!app.ShouldSkipHealthCheck())
            {
                // All health checks must pass for app to be considered ready to accept traffic after starting
                app.MapHealthChecks("/health");

                // Only health checks tagged with the "live" tag must pass for app to be considered alive
                app.MapHealthChecks("/alive", new HealthCheckOptions
                {
                    Predicate = r => r.Tags.Contains("live")
                });
            }

            app.UseSwagger();
            app.UseSwaggerUI();            
        }

        app.UseHttpsRedirection();
        
        return app;
    }
}
