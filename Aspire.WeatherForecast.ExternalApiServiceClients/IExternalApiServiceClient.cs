namespace Aspire.WeatherForecast.ExternalApiServiceClients;

public interface IClient
{
    public Task<string> GetAsync(string caller, CancellationToken cancellationToken = default);
}