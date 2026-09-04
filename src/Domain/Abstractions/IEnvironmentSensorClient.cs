using EnvLogger.Domain.Entities;

namespace EnvLogger.Domain.Abstractions;

/// <summary>
/// 環境計測端末のREST APIと通信するクライアントを表します。
/// </summary>
public interface IEnvironmentSensorClient
{
    /// <summary>
    /// 環境計測端末から現在の環境値を取得します(GET /current)。
    /// </summary>
    /// <param name="cancellationToken">キャンセルトークン。</param>
    Task<SensorSnapshot> GetCurrentAsync(CancellationToken cancellationToken = default);
}
