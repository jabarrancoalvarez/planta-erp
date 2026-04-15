using PlanTA.SharedKernel;

namespace PlanTA.Seguridad.Application.Interfaces;

public interface IEmpresaService
{
    Task<Result<bool>> CompletarOnboardingAsync(Guid empresaId, CancellationToken ct = default);
}
