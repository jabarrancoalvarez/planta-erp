using MediatR;
using Microsoft.EntityFrameworkCore;
using PlanTA.Calidad.Application.Interfaces;
using PlanTA.Calidad.Domain.Entities;
using PlanTA.Calidad.Domain.Errors;
using PlanTA.SharedKernel;

namespace PlanTA.Calidad.Application.Features.Inspecciones.RegistrarResultado;

public sealed class RegistrarResultadoCommandHandler(
    ICalidadDbContext db,
    ICurrentTenant tenant)
    : IRequestHandler<RegistrarResultadoCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(RegistrarResultadoCommand request, CancellationToken cancellationToken)
    {
        var inspeccion = await db.Inspecciones
            .Include(i => i.Resultados)
            .Where(i => i.Id == new InspeccionId(request.InspeccionId) && i.EmpresaId == tenant.EmpresaId)
            .FirstOrDefaultAsync(cancellationToken);

        if (inspeccion is null)
            return Result<bool>.Failure(InspeccionErrors.NotFound(request.InspeccionId));

        var result = inspeccion.RegistrarResultado(
            new CriterioInspeccionId(request.CriterioInspeccionId),
            request.ValorMedido, request.Cumple, request.Observaciones);

        if (result.IsFailure)
            return result;

        await db.SaveChangesAsync(cancellationToken);
        return Result<bool>.Success(true);
    }
}
