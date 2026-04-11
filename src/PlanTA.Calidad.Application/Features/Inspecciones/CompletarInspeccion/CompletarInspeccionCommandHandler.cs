using MediatR;
using Microsoft.EntityFrameworkCore;
using PlanTA.Calidad.Application.Interfaces;
using PlanTA.Calidad.Domain.Entities;
using PlanTA.Calidad.Domain.Enums;
using PlanTA.Calidad.Domain.Errors;
using PlanTA.SharedKernel;

namespace PlanTA.Calidad.Application.Features.Inspecciones.CompletarInspeccion;

public sealed class CompletarInspeccionCommandHandler(
    ICalidadDbContext db,
    ICurrentTenant tenant)
    : IRequestHandler<CompletarInspeccionCommand, Result<ResultadoInspeccion>>
{
    public async Task<Result<ResultadoInspeccion>> Handle(
        CompletarInspeccionCommand request, CancellationToken cancellationToken)
    {
        var inspeccion = await db.Inspecciones
            .Include(i => i.Resultados)
            .Where(i => i.Id == new InspeccionId(request.InspeccionId) && i.EmpresaId == tenant.EmpresaId)
            .FirstOrDefaultAsync(cancellationToken);

        if (inspeccion is null)
            return Result<ResultadoInspeccion>.Failure(InspeccionErrors.NotFound(request.InspeccionId));

        var result = inspeccion.Completar();

        if (result.IsFailure)
            return result;

        await db.SaveChangesAsync(cancellationToken);
        return result;
    }
}
