namespace PlanTA.Inventario.Application.DTOs;

public record AlertaStockDto(
    Guid Id,
    Guid ProductoId,
    Guid? AlmacenId,
    decimal StockMinimo,
    decimal StockMaximo,
    decimal PuntoReorden,
    decimal CantidadReorden,
    bool AutoReorden,
    bool Activa);
