using System.Text.Json.Serialization;

namespace EnvLogger.Application.Dtos;

/// <summary>
/// GET /api/current のレスポンスを表します。
/// </summary>
/// <param name="Temp">温度。</param>
/// <param name="Pressure">気圧。</param>
/// <param name="Humidity">湿度。</param>
/// <param name="CpuTemp">計測端末のCPU温度。</param>
public sealed record CurrentResponse(
    decimal Temp,
    decimal Pressure,
    decimal Humidity,
    [property: JsonPropertyName("cpu_temp")] decimal CpuTemp);