using EnvLogger.Application.Services;
using EnvLogger.Domain.Enums;

namespace EnvLogger.Api.Endpoints;

/// <summary>
/// 環境モニターの REST API エンドポイントを登録します。
/// </summary>
public static class EnvironmentEndpoints
{
    /// <summary>
    /// 環境モニターの REST API を登録します。
    /// </summary>
    /// <remarks>
    /// 次のエンドポイントを登録します。
    /// <list type="bullet">
    /// <item><description>GET /api/health: 稼働状態。</description></item>
    /// <item><description>GET /api/recent: 最新値から遡った環境計測履歴。</description></item>
    /// <item><description>GET /api/history: 指定期間の環境計測履歴。</description></item>
    /// <item><description>GET /api/current: 現在の環境値。</description></item>
    /// </list>
    /// </remarks>
    /// <param name="endpoints">エンドポイントの登録先。</param>
    /// <returns>エンドポイント登録後のルートビルダー。</returns>
    public static IEndpointRouteBuilder MapEnvironmentEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/api/health", () => Results.Ok(new { status = "ok" }))
            .WithName("GetHealth");

        endpoints.MapGet("/api/recent", async (int? count, int? mode, IEnvironmentQueryService queryService, CancellationToken cancellationToken) =>
        {
            var response = await queryService.GetRecentAsync(count ?? 100, (QueryMode)(mode ?? 0), cancellationToken);
            return Results.Ok(response);
        })
            .WithName("GetRecent");

        endpoints.MapGet("/api/history", async (DateTimeOffset start, DateTimeOffset end, int? mode, IEnvironmentQueryService queryService, CancellationToken cancellationToken) =>
        {
            var response = await queryService.GetHistoryAsync(start, end, (QueryMode)(mode ?? 0), cancellationToken);
            return Results.Ok(response);
        })
            .WithName("GetHistory");

        endpoints.MapGet("/api/current", async (IEnvironmentCurrentService currentService, CancellationToken cancellationToken) =>
        {
            var response = await currentService.GetCurrentAsync(cancellationToken);
            return Results.Ok(response);
        })
            .WithName("GetCurrent");

        return endpoints;
    }
}