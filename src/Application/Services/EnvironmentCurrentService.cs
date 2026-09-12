using EnvLogger.Application.Dtos;
using EnvLogger.Domain.Abstractions;

namespace EnvLogger.Application.Services;

/// <inheritdoc cref="IEnvironmentCurrentService" />
public sealed class EnvironmentCurrentService : IEnvironmentCurrentService
{
    private readonly IEnvironmentSensorClient _sensorClient;

    /// <summary>
    /// <see cref="EnvironmentCurrentService"/> の新しいインスタンスを初期化します。
    /// </summary>
    /// <param name="sensorClient">環境計測端末クライアント。</param>
    public EnvironmentCurrentService(IEnvironmentSensorClient sensorClient)
    {
        _sensorClient = sensorClient;
    }

    /// <inheritdoc />
    public async Task<CurrentResponse> GetCurrentAsync(CancellationToken cancellationToken = default)
    {
        var snapshot = await _sensorClient.GetCurrentAsync(cancellationToken).ConfigureAwait(false);
        return new CurrentResponse(snapshot.TempC, snapshot.Pressure, snapshot.Humidity, snapshot.CpuTempC);
    }
}