using PlanTA.Inventario.Domain.Entities;
using PlanTA.SharedKernel.CQRS;

namespace PlanTA.Inventario.Domain.Events;

public sealed record ProductoCreadoEvent(ProductoId ProductoId, string SKU) : IDomainEvent;

public sealed record StockActualizadoEvent(
    ProductoId ProductoId,
    AlmacenId AlmacenId,
    UbicacionId? UbicacionId,
    decimal CantidadAnterior,
    decimal CantidadNueva) : IDomainEvent;

public sealed record LoteCreadoEvent(LoteId LoteId, ProductoId ProductoId, decimal Cantidad) : IDomainEvent;

public sealed record LoteBloqueadoEvent(LoteId LoteId, string Motivo) : IDomainEvent;

public sealed record StockBajoMinimoEvent(
    ProductoId ProductoId,
    AlmacenId AlmacenId,
    decimal CantidadActual,
    decimal Minimo) : IDomainEvent;
