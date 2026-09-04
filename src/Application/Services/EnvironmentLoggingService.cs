using EnvLogger.Domain.Abstractions;
using EnvLogger.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace EnvLogger.Application.Services;

/// <inheritdoc cref="IEnvironmentLoggingService" />
public sealed class EnvironmentLoggingService : IEnvironmentLoggingService
{
    private readonly IEnvironmentSensorClient _sensorClient;
    private readonly IEnvironmentReadingRepository _repository;
    private readonly ILogger<EnvironmentLoggingService> _logger;

    /// <summary>
    /// <see cref="EnvironmentLoggingService"/> の新しいインスタンスを初期化します。
    /// </summary>
    /// <param name="sensorClient">環境計測端末クライアント。</param>
    /// <param name="repository">環境計測データリポジトリ。</param>
    /// <param name="logger">ロガー。</param>
    public EnvironmentLoggingService(
        IEnvironmentSensorClient sensorClient,
        IEnvironmentReadingRepository repository,
        ILogger<EnvironmentLoggingService> logger)
    {
        _sensorClient = sensorClient;
        _repository = repository;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task LogCurrentReadingAsync(CancellationToken cancellationToken = default)
    {
        var snapshot = await _sensorClient.GetCurrentAsync(cancellationToken).ConfigureAwait(false);

        // DB主キーとなる日時は秒以下を切り捨てたUTC時刻とする
        var timeUtc = TruncateToMinute(DateTime.UtcNow);
        var reading = new EnvironmentReading(timeUtc, snapshot.TempC, snapshot.Pressure, snapshot.Humidity, snapshot.CpuTempC);

        await _repository.SaveAsync(reading, cancellationToken).ConfigureAwait(false);
        _logger.LogInformation("環境計測値を記録しました。Time={Time}, TempC={TempC}", reading.TimeUtc, reading.TempC);
    }

    private static DateTime TruncateToMinute(DateTime value) =>
        new(value.Year, value.Month, value.Day, value.Hour, value.Minute, 0, value.Kind);
}
