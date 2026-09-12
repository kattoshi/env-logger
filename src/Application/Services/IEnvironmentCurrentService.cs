using EnvLogger.Application.Dtos;

namespace EnvLogger.Application.Services;

/// <summary>
/// 現在の環境値を取得するユースケースを表します。
/// </summary>
public interface IEnvironmentCurrentService
{
    /// <summary>
    /// 環境計測端末から現在の環境値を取得します。
    /// </summary>
    /// <param name="cancellationToken">キャンセルトークン。</param>
    Task<CurrentResponse> GetCurrentAsync(CancellationToken cancellationToken = default);
}