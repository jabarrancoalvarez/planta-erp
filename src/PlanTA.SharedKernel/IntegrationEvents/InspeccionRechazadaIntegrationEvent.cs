using PlanTA.SharedKernel.CQRS;

namespace PlanTA.SharedKernel.IntegrationEvents;

/// <summary>
/// Calidad -> Inventario: inspeccion rechazada, bloquear lote afectado.
/// </summary>
public sealed record InspeccionRechazadaIntegrationEvent(
    Guid InspeccionId,
    Guid LoteId) : IDomainEvent;
