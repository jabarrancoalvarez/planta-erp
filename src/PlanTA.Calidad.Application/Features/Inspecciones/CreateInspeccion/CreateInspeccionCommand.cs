using PlanTA.Calidad.Domain.Enums;
using PlanTA.SharedKernel.CQRS;

namespace PlanTA.Calidad.Application.Features.Inspecciones.CreateInspeccion;

public record CreateInspeccionCommand(
    Guid PlantillaInspeccionId,
    OrigenInspeccion OrigenInspeccion,
    Guid ReferenciaOrigenId,
    Guid? LoteId = null,
    string? InspectorUserId = null,
    string? Observaciones = null) : ICommand<Guid>;
