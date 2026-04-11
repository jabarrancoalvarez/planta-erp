using PlanTA.SharedKernel.CQRS;

namespace PlanTA.SharedKernel.IntegrationEvents;

/// <summary>
/// Produccion -> Inventario: producto terminado, crear lote + movimiento entrada.
/// </summary>
public sealed record ProduccionRegistradaIntegrationEvent(
    Guid OrdenFabricacionId,
    decimal Cantidad,
    Guid? LoteGeneradoId) : IDomainEvent;
