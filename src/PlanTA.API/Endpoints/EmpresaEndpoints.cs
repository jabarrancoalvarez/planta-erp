using MediatR;
using Microsoft.AspNetCore.Authorization;
using PlanTA.Seguridad.Application.Features.Empresas.CompletarOnboarding;
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
    }
}
