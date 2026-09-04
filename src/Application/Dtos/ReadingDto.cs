namespace EnvLogger.Application.Dtos;

/// <summary>
/// 最新値・履歴取得APIで返却する1件分の環境計測データを表します。
/// </summary>
/// <param name="Ts">対象日付時刻(ローカルタイム)。</param>
/// <param name="T">温度。</param>
/// <param name="P">気圧。</param>
/// <param name="H">湿度。</param>
/// <param name="Ct">CPU温度。</param>
public sealed record ReadingDto(DateTimeOffset Ts, decimal T, decimal P, decimal H, decimal Ct);
