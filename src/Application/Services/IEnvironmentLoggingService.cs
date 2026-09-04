namespace EnvLogger.Application.Services;

/// <summary>
/// 環境計測端末から現在値を取得し、DBへ記録するユースケースを表します。
/// </summary>
public interface IEnvironmentLoggingService
{
    /// <summary>
    /// 環境計測端末から現在値を取得し、DBへ1件保存します。
    /// </summary>
    /// <param name="cancellationToken">キャンセルトークン。</param>
    Task LogCurrentReadingAsync(CancellationToken cancellationToken = default);
}
