using Aspire.WeatherForecast.ApiService;
using Aspire.WeatherForecast.ExternalApiServiceClients;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire components.
builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddProblemDetails();

// Add clients for external dependencies
builder.AddExternalApiClients();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseExceptionHandler();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", async () =>
{
    app.LookForChaosIfEnabled();

    // Call external dependencies
    var clientA = app.Services.GetRequiredService<ExternalApiServiceAClient>();
    var responseA = await clientA.GetAsync("apiservice");
    app.Logger.LogInformation("Response from external A: {response}", responseA);

    var clientB = app.Services.GetRequiredService<ExternalApiServiceBClient>();
    var responseB = await clientB.GetAsync("apiservice");
    app.Logger.LogInformation("Response from external B: {response}", responseB);    

    var forecast = Enumerable.Range(1, 10).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
});

app.MapDefaultEndpoints();

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
