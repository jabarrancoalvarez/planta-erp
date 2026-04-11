using PlanTA.Compras.Domain.Entities;
using PlanTA.SharedKernel.CQRS;

namespace PlanTA.Compras.Domain.Events;

public sealed record OCEnviadaEvent(
    OrdenCompraId OrdenCompraId,
    ProveedorId ProveedorId) : IDomainEvent;

public sealed record RecepcionRegistradaEvent(
    RecepcionId RecepcionId,
    OrdenCompraId OrdenCompraId,
    List<LineaRecepcionInfo> Lineas) : IDomainEvent;

public sealed record RecepcionAceptadaEvent(
    RecepcionId RecepcionId) : IDomainEvent;

public sealed record OCCompletadaEvent(
    OrdenCompraId OrdenCompraId) : IDomainEvent;

public record LineaRecepcionInfo(
    Guid ProductoId,
    decimal CantidadRecibida,
    Guid? LoteId,
    Guid? UbicacionDestinoId);
