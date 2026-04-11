using PlanTA.Compras.Domain.Enums;

namespace PlanTA.Compras.Application.DTOs;

public record RecepcionDetailDto(
    Guid Id,
    Guid OrdenCompraId,
    string CodigoOC,
    DateTimeOffset FechaRecepcion,
    string? NumeroAlbaran,
    EstadoRecepcion EstadoRecepcion,
    string? Observaciones,
    DateTimeOffset CreatedAt,
    List<LineaRecepcionDto> Lineas);

public record RecepcionListDto(
    Guid Id,
    Guid OrdenCompraId,
    string CodigoOC,
    DateTimeOffset FechaRecepcion,
    string? NumeroAlbaran,
    EstadoRecepcion EstadoRecepcion);

public record LineaRecepcionDto(
    Guid Id,
    Guid LineaOrdenCompraId,
    Guid ProductoId,
    decimal CantidadRecibida,
    Guid? LoteId,
    Guid? UbicacionDestinoId);
