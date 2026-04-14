using MediatR;
using Microsoft.EntityFrameworkCore;
using PlanTA.RRHH.Application.Interfaces;
using PlanTA.RRHH.Domain.Entities;
using PlanTA.RRHH.Domain.Errors;
using PlanTA.SharedKernel;

namespace PlanTA.RRHH.Application.Features.Ausencias.AprobarAusencia;

public sealed class AprobarAusenciaCommandHandler(
    IRRHHDbContext db,
    ICurrentTenant tenant)
    : IRequestHandler<AprobarAusenciaCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(AprobarAusenciaCommand request, CancellationToken ct)
    {
        var id = new AusenciaId(request.Id);
        var ausencia = await db.Ausencias
            .FirstOrDefaultAsync(a => a.Id == id && a.EmpresaId == tenant.EmpresaId, ct);

        if (ausencia is null)
            return Result<bool>.Failure(AusenciaErrors.NotFound(request.Id));

        if (request.Aprobar) ausencia.Aprobar(request.UserId);
        else ausencia.Rechazar(request.UserId);

        await db.SaveChangesAsync(ct);
        return Result<bool>.Success(true);
    }
}
