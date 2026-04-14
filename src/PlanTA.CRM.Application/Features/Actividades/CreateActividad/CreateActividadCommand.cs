using PlanTA.CRM.Domain.Enums;
using PlanTA.SharedKernel.CQRS;

namespace PlanTA.CRM.Application.Features.Actividades.CreateActividad;

public record CreateActividadCommand(
    TipoActividadCrm Tipo, string Asunto, Guid UsuarioId,
    string? Detalle = null, Guid? LeadId = null, Guid? OportunidadId = null,
    DateTimeOffset? VencimientoEn = null) : ICommand<Guid>;
