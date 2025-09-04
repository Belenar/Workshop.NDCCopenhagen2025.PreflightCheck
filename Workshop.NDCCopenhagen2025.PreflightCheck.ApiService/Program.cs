using Marten;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire client integrations.
builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddProblemDetails();

builder.AddNpgsqlDataSource("marten-db");

builder.Services.AddMarten(opts =>
{
    opts.DatabaseSchemaName = "preflight";
}).UseNpgsqlDataSource();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseExceptionHandler();

string[] summaries = ["Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"];

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");

app.MapDefaultEndpoints();

var store = app.Services.GetRequiredService<IDocumentStore>();
var session = store.IdentitySession();
session.Events.Append(Guid.NewGuid(), new WeatherForecast
(
    DateOnly.FromDateTime(DateTime.Now),
    Random.Shared.Next(-20, 55),
    summaries[Random.Shared.Next(summaries.Length)]
));
await session.SaveChangesAsync();

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
