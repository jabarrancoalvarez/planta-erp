using MediatR;
using Microsoft.EntityFrameworkCore;
using PlanTA.RRHH.Application.Interfaces;
using PlanTA.RRHH.Domain.Entities;
using PlanTA.RRHH.Domain.Errors;
using PlanTA.SharedKernel;

namespace PlanTA.RRHH.Application.Features.Ausencias.DeleteAusencia;

public sealed class DeleteAusenciaCommandHandler(
    IRRHHDbContext db,
    ICurrentTenant tenant)
    : IRequestHandler<DeleteAusenciaCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(DeleteAusenciaCommand request, CancellationToken ct)
    {
        var aus = await db.Ausencias
            .FirstOrDefaultAsync(a => a.Id == new AusenciaId(request.AusenciaId) && a.EmpresaId == tenant.EmpresaId, ct);
        if (aus is null)
            return Result<bool>.Failure(AusenciaErrors.NotFound(request.AusenciaId));
        aus.SoftDelete();
        await db.SaveChangesAsync(ct);
        return Result<bool>.Success(true);
    }
}
