using EnvLogger.Domain.Entities;
using EnvLogger.Domain.Enums;

namespace EnvLogger.Domain.Abstractions;

/// <summary>
/// 環境計測データの永続化と問い合わせを行うリポジトリを表します。
/// </summary>
public interface IEnvironmentReadingRepository
{
    /// <summary>
    /// 環境計測データを1件保存します。同一日時のレコードが既に存在する場合は上書きします。
    /// </summary>
    /// <param name="reading">保存する環境計測データ。</param>
    /// <param name="cancellationToken">キャンセルトークン。</param>
    Task SaveAsync(EnvironmentReading reading, CancellationToken cancellationToken = default);

    /// <summary>
    /// 最新の値から遡って指定件数の環境計測データを取得します。
    /// </summary>
    /// <param name="count">取得件数。</param>
    /// <param name="mode">抽出モード。</param>
    /// <param name="cancellationToken">キャンセルトークン。</param>
    /// <returns>日時の降順に並んだ環境計測データ一覧。</returns>
    Task<IReadOnlyList<EnvironmentReading>> GetRecentAsync(int count, QueryMode mode, CancellationToken cancellationToken = default);

    /// <summary>
    /// 指定した期間内(UTC)の環境計測データを取得します。
    /// </summary>
    /// <param name="startUtc">期間開始日時(UTC)。</param>
    /// <param name="endUtc">期間終了日時(UTC)。</param>
    /// <param name="mode">抽出モード。</param>
    /// <param name="cancellationToken">キャンセルトークン。</param>
    /// <returns>日時の降順に並んだ環境計測データ一覧。</returns>
    Task<IReadOnlyList<EnvironmentReading>> GetHistoryAsync(DateTime startUtc, DateTime endUtc, QueryMode mode, CancellationToken cancellationToken = default);
}
