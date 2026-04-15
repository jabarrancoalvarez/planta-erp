using Microsoft.EntityFrameworkCore;
using PlanTA.Seguridad.Application.Interfaces;
using PlanTA.Seguridad.Domain.Entities;
using PlanTA.Seguridad.Infrastructure.Data;
using PlanTA.SharedKernel;

namespace PlanTA.Seguridad.Infrastructure.Services;

public sealed class EmpresaService(SeguridadDbContext db) : IEmpresaService
{
    public async Task<Result<bool>> CompletarOnboardingAsync(Guid empresaId, CancellationToken ct = default)
    {
        var id = new EmpresaId(empresaId);
        var empresa = await db.Empresas.FirstOrDefaultAsync(e => e.Id == id, ct);
        if (empresa is null)
            return Result<bool>.Failure(Error.NotFound("Empresa.NotFound", "Empresa no encontrada"));

        empresa.CompletarOnboarding();
        await db.SaveChangesAsync(ct);
        return Result<bool>.Success(true);
    }
}
