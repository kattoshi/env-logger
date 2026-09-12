using EnvLogger.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace EnvLogger.Application.DependencyInjection;

/// <summary>
/// Application層のユースケース実装をDIコンテナへ登録する拡張メソッドを提供します。
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// ロギング、最新値取得、履歴問い合わせの各ユースケース実装をDIコンテナへ登録します。
    /// </summary>
    /// <param name="services">登録先のサービスコレクション。</param>
    public static IServiceCollection AddEnvLoggerApplication(this IServiceCollection services)
    {
        services.AddScoped<IEnvironmentLoggingService, EnvironmentLoggingService>();
        services.AddScoped<IEnvironmentQueryService, EnvironmentQueryService>();
        services.AddScoped<IEnvironmentCurrentService, EnvironmentCurrentService>();
        return services;
    }
}
