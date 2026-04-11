using PlanTA.Ventas.Domain.Entities;
using PlanTA.SharedKernel.CQRS;

namespace PlanTA.Ventas.Domain.Events;

public sealed record PedidoConfirmadoEvent(
    PedidoId PedidoId,
    ClienteId ClienteId) : IDomainEvent;

public sealed record ExpedicionPreparadaEvent(
    ExpedicionId ExpedicionId,
    PedidoId PedidoId) : IDomainEvent;

public sealed record ExpedicionEnviadaEvent(
    ExpedicionId ExpedicionId,
    PedidoId PedidoId,
    List<LineaExpedicionInfo> Lineas) : IDomainEvent;

public sealed record PedidoEntregadoEvent(
    PedidoId PedidoId) : IDomainEvent;

public record LineaExpedicionInfo(
    Guid ProductoId,
    decimal Cantidad,
    Guid? LoteOrigenId);
