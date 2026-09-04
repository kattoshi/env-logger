using EnvLogger.Api.BackgroundServices;
using EnvLogger.Application.DependencyInjection;
using EnvLogger.Application.Services;
using EnvLogger.Domain.Enums;
using EnvLogger.Infrastructure.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddEnvLoggerApplication();
builder.Services.AddEnvLoggerInfrastructure(builder.Configuration);
builder.Services.AddHostedService<LoggerBackgroundService>();
builder.Services.AddHostedService<LcdDisplayBackgroundService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapGet("/api/health", () => Results.Ok(new { status = "ok" }))
    .WithName("GetHealth");

app.MapGet("/api/recent", async (int? count, int? mode, IEnvironmentQueryService queryService, CancellationToken cancellationToken) =>
{
    var response = await queryService.GetRecentAsync(count ?? 100, (QueryMode)(mode ?? 0), cancellationToken);
    return Results.Ok(response);
})
    .WithName("GetRecent");

app.MapGet("/api/history", async (DateTimeOffset start, DateTimeOffset end, int? mode, IEnvironmentQueryService queryService, CancellationToken cancellationToken) =>
{
    var response = await queryService.GetHistoryAsync(start, end, (QueryMode)(mode ?? 0), cancellationToken);
    return Results.Ok(response);
})
    .WithName("GetHistory");

app.Run();
