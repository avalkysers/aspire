namespace Aspire.WeatherForecast.ExternalApiServiceClients;

public class ExternalApiServiceBClient(HttpClient httpClient) : ExternalApiServiceClient(httpClient)
{
}