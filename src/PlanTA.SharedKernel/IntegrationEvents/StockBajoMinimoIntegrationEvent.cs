using PlanTA.SharedKernel.CQRS;

namespace PlanTA.SharedKernel.IntegrationEvents;

/// <summary>
/// Inventario -> Compras: stock bajo minimo, puede disparar orden de compra automatica.
/// </summary>
public sealed record StockBajoMinimoIntegrationEvent(
    Guid ProductoId,
    Guid AlmacenId,
    decimal CantidadActual,
    decimal Minimo) : IDomainEvent;
