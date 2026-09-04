using EnvLogger.Domain.Abstractions;
using EnvLogger.Infrastructure.Display;
using EnvLogger.Infrastructure.Options;
using EnvLogger.Infrastructure.Persistence;
using EnvLogger.Infrastructure.Sensors;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace EnvLogger.Infrastructure.DependencyInjection;

/// <summary>
/// Infrastructure層の実装をDIコンテナへ登録する拡張メソッドを提供します。
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// SQLiteリポジトリ、環境計測端末HTTPクライアント、LCDディスプレイを登録します。
    /// </summary>
    /// <param name="services">登録先のサービスコレクション。</param>
    /// <param name="configuration">設定値の取得元となる構成。</param>
    public static IServiceCollection AddEnvLoggerInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<EnvLoggerOptions>(configuration.GetSection(EnvLoggerOptions.SectionName));

        services.AddSingleton<IEnvironmentReadingRepository, SqliteEnvironmentReadingRepository>();
        services.AddSingleton<ILcdDisplay, Hd44780I2cLcdDisplay>();

        services.AddHttpClient<IEnvironmentSensorClient, HttpEnvironmentSensorClient>((sp, client) =>
        {
            var options = sp.GetRequiredService<IOptions<EnvLoggerOptions>>().Value;
            client.BaseAddress = new Uri(options.SensorBaseUrl);
            client.Timeout = TimeSpan.FromSeconds(10);
        });

        return services;
    }
}
