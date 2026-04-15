using MediatR;
using PlanTA.Seguridad.Application.Interfaces;
using PlanTA.SharedKernel;

namespace PlanTA.Seguridad.Application.Features.Empresas.CompletarOnboarding;

public sealed class CompletarOnboardingCommandHandler(IEmpresaService empresas, ICurrentTenant tenant)
    : IRequestHandler<CompletarOnboardingCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(CompletarOnboardingCommand request, CancellationToken ct)
    {
        return await empresas.CompletarOnboardingAsync(tenant.EmpresaId, ct);
    }
}
