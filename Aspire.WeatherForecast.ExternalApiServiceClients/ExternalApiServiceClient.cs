namespace Aspire.WeatherForecast.ExternalApiServiceClients;

public abstract class ExternalApiServiceClient(HttpClient httpClient) : IClient
{
    public async Task<string> GetAsync(string caller, CancellationToken cancellationToken = default)
    {
        return await httpClient.GetStringAsync($"/{caller}", cancellationToken) ?? "Nothing from external";
    }
}