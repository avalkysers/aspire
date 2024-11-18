using Aspire.WeatherForecast.ExternalApiServiceClients;

namespace Aspire.WeatherForecast.ExternalApiServiceA;

public static class Extensions
{
    public static IHostApplicationBuilder AddServices(this IHostApplicationBuilder builder)
    {
        // Load global settings from apphost for show-casing...
        builder.Configuration.AddJsonFile(Path.Combine(AppContext.BaseDirectory, "aspirehosting.appsettings.json"), optional: false);

        if(builder.Configuration["ASPIRE:ADD_SERVICE_DEFAULTS_TO_EXTERNAL_A"] == "true")
        {
            builder.AddServiceDefaults();
        }
        else
        {
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
        }

        return builder;
    }

    public static IHostApplicationBuilder AddExternalApiClients(this IHostApplicationBuilder builder)
    {
        var baseAddressB = builder.Configuration["ExternalApiServices:BaseAddressB"]!.ToString();
        
        if(builder.Configuration["ADD_SERVICE_DEFAULTS_TO_EXTERNAL_A"] == "true" &&
            !builder.ShouldSkipAspireServiceDiscovery())
        {
            baseAddressB = "https+http://external-b";
        }
        
        builder.Services.AddHttpClient<ExternalApiServiceBClient>(client =>
            {
                client.BaseAddress = new(baseAddressB!);
            });

        return builder;
    }

    public static WebApplication UseServices(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        
        return app;
    }
}