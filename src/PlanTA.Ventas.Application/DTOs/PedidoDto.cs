using PlanTA.Ventas.Domain.Enums;

namespace PlanTA.Ventas.Application.DTOs;

public record PedidoDetailDto(
    Guid Id,
    string Codigo,
    Guid ClienteId,
    string ClienteRazonSocial,
    DateTimeOffset FechaEmision,
    DateTimeOffset? FechaEntregaEstimada,
    EstadoPedido EstadoPedido,
    string? DireccionEntrega,
    string? Observaciones,
    decimal Total,
    DateTimeOffset CreatedAt,
    List<LineaPedidoDto> Lineas);

public record PedidoListDto(
    Guid Id,
    string Codigo,
    Guid ClienteId,
    string ClienteRazonSocial,
    DateTimeOffset FechaEmision,
    EstadoPedido EstadoPedido,
    decimal Total);

public record LineaPedidoDto(
    Guid Id,
    Guid ProductoId,
    string Descripcion,
    decimal Cantidad,
    decimal PrecioUnitario,
    decimal Descuento,
    decimal CantidadEnviada,
    decimal Total);
