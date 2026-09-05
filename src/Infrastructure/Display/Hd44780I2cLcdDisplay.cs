using System.Device.I2c;
using EnvLogger.Domain.Abstractions;
using EnvLogger.Domain.Entities;
using EnvLogger.Infrastructure.Options;
using Iot.Device.CharacterLcd;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace EnvLogger.Infrastructure.Display;

/// <inheritdoc cref="ILcdDisplay" />
public sealed class Hd44780I2cLcdDisplay : ILcdDisplay, IDisposable
{
    private readonly ILogger<Hd44780I2cLcdDisplay> _logger;
    private readonly I2cDevice? _i2cDevice;
    private readonly Lcd1602? _lcd;

    /// <summary>
    /// <see cref="Hd44780I2cLcdDisplay"/> の新しいインスタンスを初期化します。
    /// I2Cデバイスに接続できない場合は表示処理を無効化し、警告ログのみ出力します(開発機など非搭載環境向け)。
    /// </summary>
    /// <param name="options">env-logger の設定値(LCDのI2Cバス・アドレスを含む)。</param>
    /// <param name="logger">ロガー。</param>
    public Hd44780I2cLcdDisplay(IOptions<EnvLoggerOptions> options, ILogger<Hd44780I2cLcdDisplay> logger)
    {
        _logger = logger;
        try
        {
            _i2cDevice = I2cDevice.Create(new I2cConnectionSettings(options.Value.LcdI2cBusId, options.Value.LcdI2cAddress));
            var lcdInterface = LcdInterface.CreateI2c(_i2cDevice, false);
            _lcd = new Lcd1602(lcdInterface) { BacklightOn = true };
            _lcd.Clear();
            // 最初の計測値表示までのつなぎ表示。Show/ShowErrorが全桁上書きするためClear不要
            _lcd.SetCursorPosition(0, 0);
            _lcd.Write("Booting...");
            _lcd.SetCursorPosition(0, 1);
            _lcd.Write("Please wait");
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "LCDディスプレイの初期化に失敗しました。表示処理は無効化されます。");
        }
    }

    /// <inheritdoc />
    public void Show(DateTime localTime, SensorSnapshot snapshot)
    {
        if (_lcd is null)
        {
            return;
        }

        // 1行目: (hh:mm)9999.9hPa / 2行目: 99.9°C  999.9%
        var line1 = $"({localTime:HH:mm}){snapshot.Pressure,6:0.0}hPa";
        // TempCは氷点下になり得るため、正の数側にリテラル空白を1つ入れて符号分の桁を確保する
        var line2 = $" {snapshot.TempC,5: 0.0;-0.0}C   {snapshot.Humidity,5:0.0}%";

        _lcd.SetCursorPosition(0, 0);
        _lcd.Write(line1);
        _lcd.SetCursorPosition(0, 1);
        _lcd.Write(line2);
    }

    /// <inheritdoc />
    public void ShowError()
    {
        if (_lcd is null)
        {
            return;
        }

        // 通常表示より短い文字列を書くため、残留文字を消すためにクリアする
        _lcd.Clear();
        _lcd.SetCursorPosition(0, 0);
        _lcd.Write("Error!! Measure");
        _lcd.SetCursorPosition(0, 1);
        _lcd.Write("Device Connect");
    }

    /// <inheritdoc />
    public void Dispose()
    {
        _lcd?.Dispose();
        _i2cDevice?.Dispose();
    }
}
