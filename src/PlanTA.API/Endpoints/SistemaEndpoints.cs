using Microsoft.Extensions.Logging;

namespace PlanTA.API.Endpoints;

public sealed class SistemaEndpoints : IEndpointGroup
{
    public string? RoutePrefix => "/api/v1/sistema";

    public void MapEndpoints(RouteGroupBuilder group)
    {
        group.MapGet("/health", () => Results.Ok(new
        {
            status = "ok",
            timestamp = DateTimeOffset.UtcNow,
            version = typeof(SistemaEndpoints).Assembly.GetName().Version?.ToString() ?? "1.0.0",
        }))
        .WithName("HealthCheck")
        .WithTags("Sistema")
        .AllowAnonymous();

        group.MapPost("/frontend-error", (FrontendErrorRequest req, ILogger<SistemaEndpoints> logger) =>
        {
            logger.LogError(
                "[FrontendError] {Message} | url={Url} | ua={UserAgent} | ts={Timestamp}\n{Stack}",
                req.Message, req.Url, req.UserAgent, req.Timestamp, req.Stack);
            return Results.Ok();
        })
        .WithName("LogFrontendError")
        .WithTags("Sistema")
        .AllowAnonymous();
    }
}

public record FrontendErrorRequest(
    string Message,
    string? Stack,
    string? Url,
    string? UserAgent,
    string? Timestamp);
