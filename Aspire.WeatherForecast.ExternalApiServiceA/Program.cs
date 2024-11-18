using Aspire.WeatherForecast.ExternalApiServiceA;
using Aspire.WeatherForecast.ExternalApiServiceClients;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults
builder.AddServices();

// Add clients for external dependencies/api services
builder.AddExternalApiClients();
    
var app = builder.Build();

// Enable services
app.UseServices();

// Count requests
var count = 0;

app.MapGet("/{caller}", async (string caller) =>
{
    app.Logger.LogInformation("ExternalApiServiceA - Number of requests: {count} - Last called by {caller}", ++count, caller ?? "N/A");
    
    app.LookForChaosIfEnabled();
    
    // Call external dependency
    var client = app.Services.GetRequiredService<ExternalApiServiceBClient>();
    var response = await client.GetAsync("external-a");
    app.Logger.LogInformation("Response from external B: {response}", response);

    return "Hello from external A!";
});

app.Run();