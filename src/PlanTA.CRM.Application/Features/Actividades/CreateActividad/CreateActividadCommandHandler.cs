using MediatR;
using PlanTA.CRM.Application.Interfaces;
using PlanTA.CRM.Domain.Entities;
using PlanTA.SharedKernel;

namespace PlanTA.CRM.Application.Features.Actividades.CreateActividad;

public sealed class CreateActividadCommandHandler(
    ICrmDbContext db,
    ICurrentTenant tenant)
    : IRequestHandler<CreateActividadCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateActividadCommand request, CancellationToken ct)
    {
        var act = ActividadCrm.Crear(
            request.Tipo, request.Asunto, request.UsuarioId, tenant.EmpresaId,
            request.Detalle, request.LeadId, request.OportunidadId, request.VencimientoEn);

        db.Actividades.Add(act);
        await db.SaveChangesAsync(ct);
        return Result<Guid>.Success(act.Id.Value);
    }
}
