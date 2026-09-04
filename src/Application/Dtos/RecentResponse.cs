namespace EnvLogger.Application.Dtos;

/// <summary>
/// GET /api/recent のレスポンスを表します。
/// </summary>
/// <param name="Values">日時の降順に並んだ環境計測データ一覧。</param>
public sealed record RecentResponse(IReadOnlyList<ReadingDto> Values);
