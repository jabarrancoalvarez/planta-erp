namespace PlanTA.API.Endpoints;

public sealed class HealthEndpoints : IEndpointGroup
{
    public string? RoutePrefix => "/api/v1/health";

    public void MapEndpoints(RouteGroupBuilder group)
    {
        group.MapGet("/", () => Results.Ok(new
        {
            Status = "Healthy",
            Timestamp = DateTimeOffset.UtcNow,
            Version = "0.1.0"
        }))
        .WithName("HealthCheck")
        .WithTags("Health")
        .AllowAnonymous();
    }
}
