using System.Net.Http.Json;
using System.Text.Json.Serialization;
using EnvLogger.Domain.Abstractions;
using EnvLogger.Domain.Entities;

namespace EnvLogger.Infrastructure.Sensors;

/// <inheritdoc cref="IEnvironmentSensorClient" />
public sealed class HttpEnvironmentSensorClient : IEnvironmentSensorClient
{
    private readonly HttpClient _httpClient;

    /// <summary>
    /// <see cref="HttpEnvironmentSensorClient"/> の新しいインスタンスを初期化します。
    /// </summary>
    /// <param name="httpClient">環境計測端末へのアクセスに使用するHTTPクライアント(BaseAddress設定済み)。</param>
    public HttpEnvironmentSensorClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    /// <inheritdoc />
    public async Task<SensorSnapshot> GetCurrentAsync(CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetFromJsonAsync<SensorCurrentResponse>("/current", cancellationToken).ConfigureAwait(false);
        if (response is null)
        {
            throw new InvalidOperationException("環境計測端末から現在値を取得できませんでした。");
        }

        return new SensorSnapshot(response.Temp, response.Pressure, response.Humidity, response.CpuTemp);
    }

    /// <summary>
    /// 環境計測端末の GET /current レスポンスに対応するJSONモデルを表します。
    /// </summary>
    private sealed class SensorCurrentResponse
    {
        /// <summary>温度。</summary>
        [JsonPropertyName("temp")]
        public decimal Temp { get; set; }

        /// <summary>気圧。</summary>
        [JsonPropertyName("pressure")]
        public decimal Pressure { get; set; }

        /// <summary>湿度。</summary>
        [JsonPropertyName("humidity")]
        public decimal Humidity { get; set; }

        /// <summary>CPU温度。</summary>
        [JsonPropertyName("cpu_temp")]
        public decimal CpuTemp { get; set; }
    }
}
