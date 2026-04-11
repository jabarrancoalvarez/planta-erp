using PlanTA.Inventario.Domain.Enums;

namespace PlanTA.Inventario.Application.DTOs;

public record MovimientoStockDto(
    Guid Id,
    Guid ProductoId,
    Guid AlmacenId,
    Guid? UbicacionId,
    Guid? LoteId,
    TipoMovimiento Tipo,
    decimal Cantidad,
    decimal CantidadAnterior,
    decimal CantidadPosterior,
    string? Referencia,
    string? Notas,
    DateTimeOffset CreatedAt);
