using EnvLogger.Application.Services;

namespace EnvLogger.Api.BackgroundServices;

/// <summary>
/// 毎時0,10,20,30,40,50分に環境計測端末から現在値を取得し、DBへ記録するバックグラウンドサービスです。
/// </summary>
public sealed class LoggerBackgroundService : BackgroundService
{
    private static readonly TimeSpan Interval = TimeSpan.FromMinutes(10);

    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<LoggerBackgroundService> _logger;

    /// <summary>
    /// <see cref="LoggerBackgroundService"/> の新しいインスタンスを初期化します。
    /// </summary>
    /// <param name="scopeFactory">スコープ付きサービス解決用ファクトリ。</param>
    /// <param name="logger">ロガー。</param>
    public LoggerBackgroundService(IServiceScopeFactory scopeFactory, ILogger<LoggerBackgroundService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    /// <inheritdoc />
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(GetDelayUntilNextTick(DateTime.UtcNow), stoppingToken).ConfigureAwait(false);
            if (stoppingToken.IsCancellationRequested)
            {
                break;
            }

            try
            {
                using var scope = _scopeFactory.CreateScope();
                var loggingService = scope.ServiceProvider.GetRequiredService<IEnvironmentLoggingService>();
                await loggingService.LogCurrentReadingAsync(stoppingToken).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "環境計測値の記録に失敗しました。");
            }
        }
    }

    /// <summary>
    /// 次の10分刻み(0,10,20,30,40,50分)の実行時刻までの待機時間を計算します。
    /// 起動直後に即時実行されないよう、必ず次の刻みまで待機します。
    /// </summary>
    private static TimeSpan GetDelayUntilNextTick(DateTime nowUtc)
    {
        // ミリ秒未満まで切り捨てるためTicks単位で丸める
        var next = new DateTime(nowUtc.Ticks - (nowUtc.Ticks % Interval.Ticks), nowUtc.Kind).Add(Interval);
        return next - nowUtc;
    }
}
