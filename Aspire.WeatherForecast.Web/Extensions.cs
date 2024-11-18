namespace Aspire.WeatherForecast.Web;

public static class Extensions
{
    public static IHostApplicationBuilder AddApiClient(this IHostApplicationBuilder builder)
    {
        // This URL uses "https+http://" to indicate HTTPS is preferred over HTTP.
        // Learn more about service discovery scheme resolution at https://aka.ms/dotnet/sdschemes.
        var baseAddress = "https+http://apiservice";

        if(builder.ShouldSkipAspireServiceDiscovery())
        {
            baseAddress = builder.Configuration["ApiService:BaseAddress"]!.ToString();
        }

        builder.Services.AddHttpClient<WeatherApiClient>(client =>
            {
                client.BaseAddress = new(baseAddress!);
            });

        return builder;
    }
}