namespace EnvLogger.Domain.Entities;

/// <summary>
/// 環境計測端末の GET /current から取得した、日時を伴わない瞬時の環境計測値を表します。
/// </summary>
/// <param name="TempC">温度(摂氏)。</param>
/// <param name="Pressure">気圧(hPa)。</param>
/// <param name="Humidity">湿度(%)。</param>
/// <param name="CpuTempC">計測端末のCPU温度(摂氏)。</param>
public sealed record SensorSnapshot(decimal TempC, decimal Pressure, decimal Humidity, decimal CpuTempC);
