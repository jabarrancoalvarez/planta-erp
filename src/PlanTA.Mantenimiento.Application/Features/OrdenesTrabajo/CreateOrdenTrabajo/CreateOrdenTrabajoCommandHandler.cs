using MediatR;
using PlanTA.Mantenimiento.Application.Interfaces;
using PlanTA.Mantenimiento.Domain.Entities;
using PlanTA.SharedKernel;

namespace PlanTA.Mantenimiento.Application.Features.OrdenesTrabajo.CreateOrdenTrabajo;

public sealed class CreateOrdenTrabajoCommandHandler(
    IMantenimientoDbContext db,
    ICurrentTenant tenant)
    : IRequestHandler<CreateOrdenTrabajoCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateOrdenTrabajoCommand request, CancellationToken cancellationToken)
    {
        var ot = OrdenTrabajo.Crear(
            request.Codigo, request.Titulo, request.Tipo, request.ActivoId,
            tenant.EmpresaId, request.Descripcion, request.Prioridad,
            request.FechaPlanificada, request.HorasEstimadas,
            request.PlanMantenimientoId, request.IncidenciaId);

        db.OrdenesTrabajo.Add(ot);
        await db.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(ot.Id.Value);
    }
}
