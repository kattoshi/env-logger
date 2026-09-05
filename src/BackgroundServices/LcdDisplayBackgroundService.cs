using EnvLogger.Domain.Abstractions;

namespace EnvLogger.Api.BackgroundServices;

/// <summary>
/// 1分ごとに環境計測端末から現在値を取得し、LCDディスプレイへ表示するバックグラウンドサービスです。
/// </summary>
public sealed class LcdDisplayBackgroundService : BackgroundService
{
    private static readonly TimeSpan Interval = TimeSpan.FromMinutes(1);

    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<LcdDisplayBackgroundService> _logger;

    /// <summary>
    /// <see cref="LcdDisplayBackgroundService"/> の新しいインスタンスを初期化します。
    /// </summary>
    /// <param name="scopeFactory">スコープ付きサービス解決用ファクトリ。</param>
    /// <param name="logger">ロガー。</param>
    public LcdDisplayBackgroundService(IServiceScopeFactory scopeFactory, ILogger<LcdDisplayBackgroundService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    /// <inheritdoc />
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = _scopeFactory.CreateScope();
            var sensorClient = scope.ServiceProvider.GetRequiredService<IEnvironmentSensorClient>();
            var lcdDisplay = scope.ServiceProvider.GetRequiredService<ILcdDisplay>();

            try
            {
                var snapshot = await sensorClient.GetCurrentAsync(stoppingToken).ConfigureAwait(false);
                lcdDisplay.Show(DateTime.Now, snapshot);
            }
            catch (HttpRequestException ex)
            {
                // 環境計測端末への接続失敗は高頻度で発生しうるため、スタックトレースなしの簡潔なログに留める
                _logger.LogWarning("環境計測端末への接続に失敗しました: {Message}", ex.Message);
                lcdDisplay.ShowError();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "LCDディスプレイへの表示に失敗しました。");
                lcdDisplay.ShowError();
            }

            try
            {
                await Task.Delay(GetDelayUntilNextMinute(DateTime.Now), stoppingToken).ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                break;
            }
        }
    }

    /// <summary>
    /// 次の毎分0秒までの待機時間を計算します。
    /// </summary>
    private static TimeSpan GetDelayUntilNextMinute(DateTime now)
    {
        // ミリ秒未満まで切り捨てるためTicks単位で丸める
        var next = new DateTime(now.Ticks - (now.Ticks % Interval.Ticks), now.Kind).Add(Interval);
        return next - now;
    }
}
