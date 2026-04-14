using MediatR;
using Microsoft.EntityFrameworkCore;
using PlanTA.RRHH.Application.Interfaces;
using PlanTA.RRHH.Domain.Entities;
using PlanTA.RRHH.Domain.Errors;
using PlanTA.SharedKernel;

namespace PlanTA.RRHH.Application.Features.Ausencias.UpdateAusencia;

public sealed class UpdateAusenciaCommandHandler(
    IRRHHDbContext db,
    ICurrentTenant tenant)
    : IRequestHandler<UpdateAusenciaCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(UpdateAusenciaCommand request, CancellationToken ct)
    {
        var aus = await db.Ausencias
            .FirstOrDefaultAsync(a => a.Id == new AusenciaId(request.AusenciaId) && a.EmpresaId == tenant.EmpresaId, ct);
        if (aus is null)
            return Result<bool>.Failure(AusenciaErrors.NotFound(request.AusenciaId));
        var result = aus.Editar(request.Tipo, request.FechaInicio, request.FechaFin, request.Motivo);
        if (result.IsFailure) return result;
        await db.SaveChangesAsync(ct);
        return Result<bool>.Success(true);
    }
}
