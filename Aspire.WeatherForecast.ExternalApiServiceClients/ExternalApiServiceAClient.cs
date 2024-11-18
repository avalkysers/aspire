namespace Aspire.WeatherForecast.ExternalApiServiceClients;

public class ExternalApiServiceAClient(HttpClient httpClient) : ExternalApiServiceClient(httpClient)
{
}