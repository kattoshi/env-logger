using System.Globalization;
using EnvLogger.Domain.Abstractions;
using EnvLogger.Domain.Entities;
using EnvLogger.Domain.Enums;
using EnvLogger.Infrastructure.Options;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Options;

namespace EnvLogger.Infrastructure.Persistence;

/// <inheritdoc cref="IEnvironmentReadingRepository" />
public sealed class SqliteEnvironmentReadingRepository : IEnvironmentReadingRepository
{
    private const string TimeFormat = "yyyy-MM-ddTHH:mm:ss";
    private readonly string _connectionString;

    /// <summary>
    /// <see cref="SqliteEnvironmentReadingRepository"/> の新しいインスタンスを初期化し、logger テーブルが存在しない場合は作成します。
    /// </summary>
    /// <param name="options">env-logger の設定値。</param>
    public SqliteEnvironmentReadingRepository(IOptions<EnvLoggerOptions> options)
    {
        var dbPath = options.Value.DbPath;
        var directory = Path.GetDirectoryName(dbPath);
        if (!string.IsNullOrEmpty(directory))
        {
            Directory.CreateDirectory(directory);
        }

        _connectionString = new SqliteConnectionStringBuilder { DataSource = dbPath }.ToString();
        EnsureTableCreated();
    }

    private void EnsureTableCreated()
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText = """
            CREATE TABLE IF NOT EXISTS logger (
                time TEXT PRIMARY KEY,
                tempC REAL NOT NULL,
                humidity REAL NOT NULL,
                pressure REAL NOT NULL,
                cpu_temp REAL NOT NULL
            );
            """;
        command.ExecuteNonQuery();
    }

    /// <inheritdoc />
    public async Task SaveAsync(EnvironmentReading reading, CancellationToken cancellationToken = default)
    {
        await using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync(cancellationToken).ConfigureAwait(false);

        await using var command = connection.CreateCommand();
        command.CommandText = """
            INSERT INTO logger (time, tempC, humidity, pressure, cpu_temp)
            VALUES ($time, $tempC, $humidity, $pressure, $cpuTemp)
            ON CONFLICT(time) DO UPDATE SET
                tempC = excluded.tempC,
                humidity = excluded.humidity,
                pressure = excluded.pressure,
                cpu_temp = excluded.cpu_temp;
            """;
        command.Parameters.AddWithValue("$time", reading.TimeUtc.ToString(TimeFormat, CultureInfo.InvariantCulture));
        command.Parameters.AddWithValue("$tempC", reading.TempC);
        command.Parameters.AddWithValue("$humidity", reading.Humidity);
        command.Parameters.AddWithValue("$pressure", reading.Pressure);
        command.Parameters.AddWithValue("$cpuTemp", reading.CpuTempC);

        await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<EnvironmentReading>> GetRecentAsync(int count, QueryMode mode, CancellationToken cancellationToken = default)
    {
        await using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync(cancellationToken).ConfigureAwait(false);

        await using var command = connection.CreateCommand();
        command.CommandText = mode == QueryMode.HourlyOnMinuteZero
            ? """
                SELECT time, tempC, humidity, pressure, cpu_temp FROM logger
                WHERE strftime('%M', time) = '00'
                ORDER BY time DESC
                LIMIT $count;
                """
            : """
                SELECT time, tempC, humidity, pressure, cpu_temp FROM logger
                ORDER BY time DESC
                LIMIT $count;
                """;
        command.Parameters.AddWithValue("$count", count);

        return await ReadAllAsync(command, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<EnvironmentReading>> GetHistoryAsync(DateTime startUtc, DateTime endUtc, QueryMode mode, CancellationToken cancellationToken = default)
    {
        await using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync(cancellationToken).ConfigureAwait(false);

        await using var command = connection.CreateCommand();
        command.CommandText = mode == QueryMode.HourlyOnMinuteZero
            ? """
                SELECT time, tempC, humidity, pressure, cpu_temp FROM logger
                WHERE time BETWEEN $start AND $end AND strftime('%M', time) = '00'
                ORDER BY time DESC;
                """
            : """
                SELECT time, tempC, humidity, pressure, cpu_temp FROM logger
                WHERE time BETWEEN $start AND $end
                ORDER BY time DESC;
                """;
        command.Parameters.AddWithValue("$start", startUtc.ToString(TimeFormat, CultureInfo.InvariantCulture));
        command.Parameters.AddWithValue("$end", endUtc.ToString(TimeFormat, CultureInfo.InvariantCulture));

        return await ReadAllAsync(command, cancellationToken).ConfigureAwait(false);
    }

    private static async Task<IReadOnlyList<EnvironmentReading>> ReadAllAsync(SqliteCommand command, CancellationToken cancellationToken)
    {
        var results = new List<EnvironmentReading>();
        await using var reader = await command.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);
        while (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
        {
            var time = DateTime.SpecifyKind(
                DateTime.ParseExact(reader.GetString(0), TimeFormat, CultureInfo.InvariantCulture),
                DateTimeKind.Utc);
            results.Add(new EnvironmentReading(
                time,
                (decimal)reader.GetDouble(1),
                (decimal)reader.GetDouble(3),
                (decimal)reader.GetDouble(2),
                (decimal)reader.GetDouble(4)));
        }

        return results;
    }
}
