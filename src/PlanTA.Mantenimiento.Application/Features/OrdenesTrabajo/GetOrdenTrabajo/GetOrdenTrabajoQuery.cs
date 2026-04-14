using MediatR;
using Microsoft.EntityFrameworkCore;
using PlanTA.Mantenimiento.Application.DTOs;
using PlanTA.Mantenimiento.Application.Interfaces;
using PlanTA.Mantenimiento.Domain.Entities;
using PlanTA.Mantenimiento.Domain.Errors;
using PlanTA.SharedKernel;
using PlanTA.SharedKernel.CQRS;

namespace PlanTA.Mantenimiento.Application.Features.OrdenesTrabajo.GetOrdenTrabajo;

public record GetOrdenTrabajoQuery(Guid OrdenTrabajoId) : IQuery<OrdenTrabajoDto>;

public sealed class GetOrdenTrabajoQueryHandler(
    IMantenimientoDbContext db,
    ICurrentTenant tenant)
    : IRequestHandler<GetOrdenTrabajoQuery, Result<OrdenTrabajoDto>>
{
    public async Task<Result<OrdenTrabajoDto>> Handle(GetOrdenTrabajoQuery request, CancellationToken cancellationToken)
    {
        var ot = await db.OrdenesTrabajo
            .Where(o => o.Id == new OrdenTrabajoId(request.OrdenTrabajoId) && o.EmpresaId == tenant.EmpresaId)
            .Select(o => new OrdenTrabajoDto(
                o.Id.Value, o.Codigo, o.Titulo, o.Descripcion, o.Tipo, o.Estado, o.Prioridad,
                o.ActivoId, o.AsignadoAUserId, o.FechaPlanificada, o.FechaInicio, o.FechaFin,
                o.HorasEstimadas, o.HorasReales, o.CosteManoObra, o.CosteRepuestos, o.NotasCierre))
            .FirstOrDefaultAsync(cancellationToken);

        return ot is null
            ? Result<OrdenTrabajoDto>.Failure(OrdenTrabajoErrors.NotFound(request.OrdenTrabajoId))
            : Result<OrdenTrabajoDto>.Success(ot);
    }
}
