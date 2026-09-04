using EnvLogger.Domain.Entities;

namespace EnvLogger.Domain.Abstractions;

/// <summary>
/// I2C接続の1602キャラクターLCDへの現在値表示を行うディスプレイを表します。
/// </summary>
public interface ILcdDisplay
{
    /// <summary>
    /// 現在時刻と環境計測値をLCDの1・2行目に表示します。
    /// </summary>
    /// <param name="localTime">表示する現在時刻(ローカルタイム)。</param>
    /// <param name="snapshot">表示する環境計測値。</param>
    void Show(DateTime localTime, SensorSnapshot snapshot);
}
