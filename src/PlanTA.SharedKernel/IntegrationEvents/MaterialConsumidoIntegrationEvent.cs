using PlanTA.SharedKernel.CQRS;

namespace PlanTA.SharedKernel.IntegrationEvents;

/// <summary>
/// Produccion -> Inventario: material consumido en una OF, registrar salida de stock.
/// </summary>
public sealed record MaterialConsumidoIntegrationEvent(
    Guid OrdenFabricacionId,
    Guid ProductoId,
    Guid? LoteId,
    decimal Cantidad) : IDomainEvent;
