using PlanTA.SharedKernel.CQRS;

namespace PlanTA.SharedKernel.IntegrationEvents;

/// <summary>
/// Compras -> Inventario: una recepcion fue aceptada, crear lote + movimiento entrada.
/// </summary>
public sealed record RecepcionAceptadaIntegrationEvent(Guid RecepcionId) : IDomainEvent;
