using Aspire.WeatherForecast.ExternalApiServiceB;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.AddServices();

var app = builder.Build();

// Enable services
app.UseServices();

// Count requests
var count = 0;

app.MapGet("/{caller}", (string caller) =>
{
    app.Logger.LogInformation("ExternalApiServiceB - Number of requests: {count} - Last called by {caller}", ++count, caller ?? "N/A");

    app.LookForChaosIfEnabled();

    return "Hello from external B!";
});

app.Run();