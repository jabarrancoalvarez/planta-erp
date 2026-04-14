using MediatR;
using PlanTA.RRHH.Application.Interfaces;
using PlanTA.RRHH.Domain.Entities;
using PlanTA.SharedKernel;

namespace PlanTA.RRHH.Application.Features.Fichajes.RegistrarFichaje;

public sealed class RegistrarFichajeCommandHandler(
    IRRHHDbContext db,
    ICurrentTenant tenant)
    : IRequestHandler<RegistrarFichajeCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(RegistrarFichajeCommand request, CancellationToken ct)
    {
        var fichaje = Fichaje.Crear(
            new EmpleadoId(request.EmpleadoId), request.Tipo, tenant.EmpresaId,
            request.ActivoId, request.OrdenFabricacionId, request.OrdenTrabajoId, request.Notas);

        db.Fichajes.Add(fichaje);
        await db.SaveChangesAsync(ct);
        return Result<Guid>.Success(fichaje.Id.Value);
    }
}
