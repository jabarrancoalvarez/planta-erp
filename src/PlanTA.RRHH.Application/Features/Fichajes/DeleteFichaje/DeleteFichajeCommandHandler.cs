using MediatR;
using Microsoft.EntityFrameworkCore;
using PlanTA.RRHH.Application.Interfaces;
using PlanTA.RRHH.Domain.Entities;
using PlanTA.RRHH.Domain.Errors;
using PlanTA.SharedKernel;

namespace PlanTA.RRHH.Application.Features.Fichajes.DeleteFichaje;

public sealed class DeleteFichajeCommandHandler(
    IRRHHDbContext db,
    ICurrentTenant tenant)
    : IRequestHandler<DeleteFichajeCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(DeleteFichajeCommand request, CancellationToken ct)
    {
        var fichaje = await db.Fichajes
            .FirstOrDefaultAsync(f => f.Id == new FichajeId(request.FichajeId) && f.EmpresaId == tenant.EmpresaId, ct);
        if (fichaje is null)
            return Result<bool>.Failure(FichajeErrors.NotFound(request.FichajeId));
        db.Fichajes.Remove(fichaje);
        await db.SaveChangesAsync(ct);
        return Result<bool>.Success(true);
    }
}
