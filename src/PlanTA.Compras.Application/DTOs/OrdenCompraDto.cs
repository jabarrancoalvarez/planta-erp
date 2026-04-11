using PlanTA.Compras.Domain.Enums;

namespace PlanTA.Compras.Application.DTOs;

public record OCDetailDto(
    Guid Id,
    string Codigo,
    Guid ProveedorId,
    string ProveedorRazonSocial,
    DateTimeOffset FechaEmision,
    DateTimeOffset? FechaEntregaEstimada,
    EstadoOC EstadoOC,
    string? Observaciones,
    decimal Total,
    DateTimeOffset CreatedAt,
    List<LineaOCDto> Lineas);

public record OCListDto(
    Guid Id,
    string Codigo,
    Guid ProveedorId,
    string ProveedorRazonSocial,
    DateTimeOffset FechaEmision,
    EstadoOC EstadoOC,
    decimal Total);

public record LineaOCDto(
    Guid Id,
    Guid ProductoId,
    string Descripcion,
    decimal Cantidad,
    decimal PrecioUnitario,
    decimal CantidadRecibida,
    decimal Total);
