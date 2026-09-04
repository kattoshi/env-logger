namespace EnvLogger.Domain.Entities;

/// <summary>
/// DBに格納された、または問い合わせ結果として返却される1件の環境計測データを表します。
/// </summary>
/// <param name="TimeUtc">計測日時(UTC、秒以下切り捨て)。</param>
/// <param name="TempC">温度(摂氏)。</param>
/// <param name="Pressure">気圧(hPa)。</param>
/// <param name="Humidity">湿度(%)。</param>
/// <param name="CpuTempC">計測端末のCPU温度(摂氏)。</param>
public sealed record EnvironmentReading(DateTime TimeUtc, decimal TempC, decimal Pressure, decimal Humidity, decimal CpuTempC);
