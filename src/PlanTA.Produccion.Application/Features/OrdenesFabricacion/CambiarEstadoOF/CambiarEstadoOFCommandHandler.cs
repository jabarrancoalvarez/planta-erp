using MediatR;
using Microsoft.EntityFrameworkCore;
using PlanTA.Produccion.Application.Interfaces;
using PlanTA.Produccion.Domain.Entities;
using PlanTA.Produccion.Domain.Enums;
using PlanTA.Produccion.Domain.Errors;
using PlanTA.SharedKernel;

namespace PlanTA.Produccion.Application.Features.OrdenesFabricacion.CambiarEstadoOF;

public sealed class CambiarEstadoOFCommandHandler(
    IProduccionDbContext db,
    ICurrentTenant tenant)
    : IRequestHandler<CambiarEstadoOFCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(CambiarEstadoOFCommand request, CancellationToken cancellationToken)
    {
        var of = await db.OrdenesFabricacion
            .Include(o => o.PartesProduccion)
            .Where(o => o.Id == new OrdenFabricacionId(request.OrdenFabricacionId) && o.EmpresaId == tenant.EmpresaId)
            .FirstOrDefaultAsync(cancellationToken);

        if (of is null)
            return Result<bool>.Failure(OrdenFabricacionErrors.NotFound(request.OrdenFabricacionId));

        var result = request.EstadoDestino switch
        {
            EstadoOF.EnPreparacion => of.Preparar(),
            EstadoOF.EnCurso when of.EstadoOF == EstadoOF.Pausada => of.Reanudar(),
            EstadoOF.EnCurso => of.Iniciar(),
            EstadoOF.Pausada => of.Pausar(),
            EstadoOF.Completada => of.Completar(),
            EstadoOF.Cancelada => of.Cancelar(request.Motivo ?? "Sin motivo especificado"),
            _ => Result<bool>.Failure(
                OrdenFabricacionErrors.TransicionInvalida(of.EstadoOF.ToString(), request.EstadoDestino.ToString()))
        };

        if (result.IsFailure)
            return result;

        await db.SaveChangesAsync(cancellationToken);
        return Result<bool>.Success(true);
    }
}
