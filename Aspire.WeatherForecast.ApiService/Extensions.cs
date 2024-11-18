using Aspire.WeatherForecast.ExternalApiServiceClients;

namespace Aspire.WeatherForecast.ApiService;

public static class Extensions
{
    public static IHostApplicationBuilder AddExternalApiClients(this IHostApplicationBuilder builder)
    {
        var baseAddressA = "https+http://external-a";
        var baseAddressB = "https+http://external-b";

        if(builder.ShouldSkipAspireServiceDiscovery())
        {
            baseAddressA = builder.Configuration["ExternalApiServices:BaseAddressA"]!.ToString();
            baseAddressB = builder.Configuration["ExternalApiServices:BaseAddressB"]!.ToString();
        }

        builder.Services.AddHttpClient<ExternalApiServiceAClient>(client =>
            {
                client.BaseAddress = new(baseAddressA!);
            });

        builder.Services.AddHttpClient<ExternalApiServiceBClient>(client =>
            {
                client.BaseAddress = new(baseAddressB!);
            });

        return builder;
    }
}