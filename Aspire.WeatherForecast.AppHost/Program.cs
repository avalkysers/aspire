using Microsoft.Extensions.Configuration;

var builder = DistributedApplication.CreateBuilder(args);
builder.Configuration.AddJsonFile("aspirehosting.appsettings.json");

var skipAspireHostingOfApiService = builder.Configuration["ASPIRE:SKIP_HOSTING_API"] == "true";
var skipAspireHostingOfExternalApiServices = builder.Configuration["ASPIRE:SKIP_HOSTING_EXTERNAL"] == "true";

if(skipAspireHostingOfExternalApiServices)
{
    if(!skipAspireHostingOfApiService)
    {
        var apiService = builder.AddProject<Projects.Aspire_WeatherForecast_ApiService>("apiservice");
        builder.AddProject<Projects.Aspire_WeatherForecast_Web>("webfrontend")
        .WithExternalHttpEndpoints()
        .WithReference(apiService);
    }
    else
    {
        builder.AddProject<Projects.Aspire_WeatherForecast_Web>("webfrontend")
        .WithExternalHttpEndpoints();
    }
}
else
{
    var externalApiServiceB = builder.AddProject<Projects.Aspire_WeatherForecast_ExternalApiServiceB>("external-b");

    var externalApiServiceA = builder.AddProject<Projects.Aspire_WeatherForecast_ExternalApiServiceA>("external-a")
        .WithReference(externalApiServiceB);

    var apiService = builder.AddProject<Projects.Aspire_WeatherForecast_ApiService>("apiservice")
        .WithReference(externalApiServiceA)
        .WithReference(externalApiServiceB);

    builder.AddProject<Projects.Aspire_WeatherForecast_Web>("webfrontend")
        .WithExternalHttpEndpoints()
        .WithReference(apiService);
}

builder.Build().Run();