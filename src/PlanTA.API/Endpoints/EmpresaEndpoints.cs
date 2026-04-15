using MediatR;
using Microsoft.AspNetCore.Authorization;
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
            EmpresaDemoSeeder seeder, ICurrentTenant tenant, IMediator m, CancellationToken ct) =>
        {
            var seed = await seeder.SeedAsync(tenant.EmpresaId, ct);
            if (!seed.IsSuccess) return seed.ToHttpResult();
            await m.Send(new CompletarOnboardingCommand(), ct);
            return Results.Ok(new { creados = seed.Value });
        })
        .WithName("CargarDatosDemo")
        .WithTags("Empresa");
    }
}
