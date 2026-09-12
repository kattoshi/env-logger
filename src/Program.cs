using EnvLogger.Api.BackgroundServices;
using EnvLogger.Api.Endpoints;
using EnvLogger.Application.DependencyInjection;
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

// デフォルトファイルと静的ファイルを配信
app.UseDefaultFiles();
app.UseStaticFiles();

app.MapEnvironmentEndpoints();

app.MapFallbackToFile("index.html");

app.Run();
