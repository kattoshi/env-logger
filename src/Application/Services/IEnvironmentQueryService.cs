using EnvLogger.Application.Dtos;
using EnvLogger.Domain.Enums;

namespace EnvLogger.Application.Services;

/// <summary>
/// 環境計測データの最新値・履歴問い合わせユースケースを表します。
/// </summary>
public interface IEnvironmentQueryService
{
    /// <summary>
    /// 最新の値から遡って指定件数の環境計測データを取得します。
    /// </summary>
    /// <param name="count">取得件数。</param>
    /// <param name="mode">抽出モード。</param>
    /// <param name="cancellationToken">キャンセルトークン。</param>
    Task<RecentResponse> GetRecentAsync(int count, QueryMode mode, CancellationToken cancellationToken = default);

    /// <summary>
    /// 指定した期間の環境計測データを取得します。
    /// </summary>
    /// <param name="start">期間開始日付時刻。</param>
    /// <param name="end">期間終了日付時刻。</param>
    /// <param name="mode">抽出モード。</param>
    /// <param name="cancellationToken">キャンセルトークン。</param>
    Task<HistoryResponse> GetHistoryAsync(DateTimeOffset start, DateTimeOffset end, QueryMode mode, CancellationToken cancellationToken = default);
}
