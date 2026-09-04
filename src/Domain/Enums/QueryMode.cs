namespace EnvLogger.Domain.Enums;

/// <summary>
/// 最新値取得・履歴取得APIにおけるレコード抽出モードを表します。
/// </summary>
public enum QueryMode
{
    /// <summary>全レコードを対象とする(既定値)。</summary>
    All = 0,

    /// <summary>毎時0分の値のみを対象とする。</summary>
    HourlyOnMinuteZero = 1,
}
