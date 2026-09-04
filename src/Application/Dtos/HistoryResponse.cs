namespace EnvLogger.Application.Dtos;

/// <summary>
/// GET /api/history のレスポンスにおける対象期間を表します。
/// </summary>
/// <param name="Start">返却開始日付時刻。</param>
/// <param name="End">返却終了日付時刻。</param>
public sealed record PeriodDto(DateTimeOffset Start, DateTimeOffset End);

/// <summary>
/// GET /api/history のレスポンスを表します。
/// </summary>
/// <param name="Period">対象期間。</param>
/// <param name="Values">日時の降順に並んだ環境計測データ一覧。</param>
public sealed record HistoryResponse(PeriodDto Period, IReadOnlyList<ReadingDto> Values);
