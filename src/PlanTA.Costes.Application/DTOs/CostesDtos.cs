using PlanTA.Costes.Domain.Enums;

namespace PlanTA.Costes.Application.DTOs;

public record ImputacionCosteDto(
    Guid Id, Guid? OrdenFabricacionId, Guid? OrdenTrabajoId, Guid? ProductoId,
    TipoCoste Tipo, OrigenImputacion Origen,
    decimal Cantidad, decimal PrecioUnitario, decimal Importe,
    string? Concepto, DateTimeOffset Fecha);

public record ResumenCosteOFDto(
    Guid OrdenFabricacionId,
    decimal TotalMaterial,
    decimal TotalManoObra,
    decimal TotalMaquina,
    decimal TotalOtros,
    decimal TotalGeneral,
    int NumeroImputaciones);

public record PagedResult<T>(List<T> Items, int TotalCount, int Page, int PageSize)
{
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
}
