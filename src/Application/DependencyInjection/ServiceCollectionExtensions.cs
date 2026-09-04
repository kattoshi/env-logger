using EnvLogger.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace EnvLogger.Application.DependencyInjection;

/// <summary>
/// Application層のユースケース実装をDIコンテナへ登録する拡張メソッドを提供します。
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// ロギングユースケース・問い合わせユースケースの実装を登録します。
    /// </summary>
    /// <param name="services">登録先のサービスコレクション。</param>
    public static IServiceCollection AddEnvLoggerApplication(this IServiceCollection services)
    {
        services.AddScoped<IEnvironmentLoggingService, EnvironmentLoggingService>();
        services.AddScoped<IEnvironmentQueryService, EnvironmentQueryService>();
        return services;
    }
}
