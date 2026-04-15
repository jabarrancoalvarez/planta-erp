using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using PlanTA.API.Services;
using PlanTA.Seguridad.Application.Features.Empresas.CompletarOnboarding;
using PlanTA.SharedKernel;
using PlanTA.SharedKernel.Extensions;

namespace PlanTA.API.Endpoints;

public sealed class EmpresaEndpoints : IEndpointGroup
{
    public string? RoutePrefix => "/api/v1/seguridad/empresa";

    public void MapEndpoints(RouteGroupBuilder group)
    {
        group.MapPost("/completar-onboarding", [Authorize] async (IMediator m, CancellationToken ct) =>
        {
            var result = await m.Send(new CompletarOnboardingCommand(), ct);
            return result.ToHttpResult();
        })
        .WithName("CompletarOnboarding")
        .WithTags("Empresa");

        group.MapPost("/cargar-datos-demo", [Authorize] async (
            EmpresaDemoSeeder seeder, ICurrentTenant tenant, IMediator m,
            ILogger<EmpresaDemoSeeder> logger, CancellationToken ct) =>
        {
            try
            {
                var seed = await seeder.SeedAsync(tenant.EmpresaId, ct);
                if (!seed.IsSuccess) return seed.ToHttpResult();
                await m.Send(new CompletarOnboardingCommand(), ct);
                return Results.Ok(new { creados = seed.Value });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error cargando datos demo para empresa {EmpresaId}", tenant.EmpresaId);
                return Results.Problem(
                    title: "Error al cargar datos demo",
                    detail: $"{ex.GetType().Name}: {ex.Message}{(ex.InnerException != null ? " | Inner: " + ex.InnerException.Message : "")}",
                    statusCode: 500);
            }
        })
        .WithName("CargarDatosDemo")
        .WithTags("Empresa");
    }
}
