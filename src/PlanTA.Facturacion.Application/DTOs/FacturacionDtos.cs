using PlanTA.Facturacion.Domain.Enums;

namespace PlanTA.Facturacion.Application.DTOs;

public record FacturaListDto(
    Guid Id, string NumeroCompleto, Guid ClienteId, string ClienteNombre,
    DateTimeOffset FechaEmision, decimal Total,
    EstadoFactura Estado, EstadoVerifactu EstadoVerifactu);

public record LineaFacturaDto(
    Guid Id, int NumeroLinea, string Descripcion,
    decimal Cantidad, decimal PrecioUnitario, decimal DescuentoPct, decimal IvaPct,
    decimal BaseImponible, decimal Iva, decimal Total, Guid? ProductoId);

public record FacturaDto(
    Guid Id, string NumeroCompleto, string SerieCodigo, int Numero, int Ejercicio,
    TipoFactura Tipo, EstadoFactura Estado,
    Guid ClienteId, string ClienteNombre, string? ClienteNIF, string? ClienteDireccion,
    DateTimeOffset FechaEmision, DateTimeOffset? FechaVencimiento,
    decimal BaseImponible, decimal TotalIva, decimal Total, string? Observaciones,
    string? HashPrevio, string? HashActual, string? CodigoQrVerifactu,
    EstadoVerifactu EstadoVerifactu, string? VerifactuCsv,
    List<LineaFacturaDto> Lineas);

public record PagedResult<T>(List<T> Items, int TotalCount, int Page, int PageSize)
{
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
}
