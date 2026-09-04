using EnvLogger.Application.Dtos;
using EnvLogger.Domain.Abstractions;
using EnvLogger.Domain.Entities;
using EnvLogger.Domain.Enums;

namespace EnvLogger.Application.Services;

/// <inheritdoc cref="IEnvironmentQueryService" />
public sealed class EnvironmentQueryService : IEnvironmentQueryService
{
    private readonly IEnvironmentReadingRepository _repository;

    /// <summary>
    /// <see cref="EnvironmentQueryService"/> の新しいインスタンスを初期化します。
    /// </summary>
    /// <param name="repository">環境計測データリポジトリ。</param>
    public EnvironmentQueryService(IEnvironmentReadingRepository repository)
    {
        _repository = repository;
    }

    /// <inheritdoc />
    public async Task<RecentResponse> GetRecentAsync(int count, QueryMode mode, CancellationToken cancellationToken = default)
    {
        var readings = await _repository.GetRecentAsync(count, mode, cancellationToken).ConfigureAwait(false);
        return new RecentResponse(readings.Select(ToDto).ToList());
    }

    /// <inheritdoc />
    public async Task<HistoryResponse> GetHistoryAsync(DateTimeOffset start, DateTimeOffset end, QueryMode mode, CancellationToken cancellationToken = default)
    {
        var readings = await _repository.GetHistoryAsync(start.UtcDateTime, end.UtcDateTime, mode, cancellationToken).ConfigureAwait(false);
        return new HistoryResponse(new PeriodDto(start, end), readings.Select(ToDto).ToList());
    }

    private static ReadingDto ToDto(EnvironmentReading reading) =>
        new(new DateTimeOffset(reading.TimeUtc, TimeSpan.Zero).ToLocalTime(), reading.TempC, reading.Pressure, reading.Humidity, reading.CpuTempC);
}
