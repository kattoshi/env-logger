namespace EnvLogger.Infrastructure.Options;

/// <summary>
/// env-logger アプリケーションの設定値を表します(appsettings.json の "EnvLogger" セクションに対応)。
/// </summary>
public sealed class EnvLoggerOptions
{
    /// <summary>
    /// 設定セクション名。
    /// </summary>
    public const string SectionName = "EnvLogger";

    /// <summary>
    /// SQLite DBファイルのパス。
    /// </summary>
    public string DbPath { get; set; } = "logger.db";

    /// <summary>
    /// 環境計測端末のベースURL(例: http://env-measure.local)。
    /// </summary>
    public string SensorBaseUrl { get; set; } = "http://env-measure.local";

    /// <summary>
    /// LCDが接続されているI2CバスID。
    /// </summary>
    public int LcdI2cBusId { get; set; } = 1;

    /// <summary>
    /// LCDのI2Cスレーブアドレス。
    /// </summary>
    public int LcdI2cAddress { get; set; } = 0x27;
}
