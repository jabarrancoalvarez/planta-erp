using PlanTA.Ventas.Domain.Enums;

namespace PlanTA.Ventas.Application.DTOs;

public record ExpedicionDetailDto(
    Guid Id,
    Guid PedidoId,
    string CodigoPedido,
    DateTimeOffset FechaExpedicion,
    string? NumeroSeguimiento,
    string? Transportista,
    EstadoExpedicion EstadoExpedicion,
    string? Observaciones,
    DateTimeOffset CreatedAt,
    List<LineaExpedicionDto> Lineas);

public record ExpedicionListDto(
    Guid Id,
    Guid PedidoId,
    string CodigoPedido,
    DateTimeOffset FechaExpedicion,
    string? NumeroSeguimiento,
    string? Transportista,
    EstadoExpedicion EstadoExpedicion);

public record LineaExpedicionDto(
    Guid Id,
    Guid LineaPedidoId,
    Guid ProductoId,
    decimal Cantidad,
    Guid? LoteOrigenId);
