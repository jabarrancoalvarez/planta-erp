namespace PlanTA.OEE.Application.DTOs;

public record RegistroOEEDto(
    Guid Id, Guid ActivoId, Guid? TurnoId, Guid? OrdenFabricacionId,
    DateTimeOffset Fecha,
    int MinutosPlanificados, int MinutosFuncionamiento,
    int PiezasTotales, int PiezasBuenas, decimal TiempoCicloTeoricoSeg,
    decimal Disponibilidad, decimal Rendimiento, decimal Calidad, decimal OEE,
    string? Notas);

public record ResumenOEEPorActivoDto(
    Guid ActivoId,
    int NumRegistros,
    decimal DisponibilidadMedia,
    decimal RendimientoMedio,
    decimal CalidadMedia,
    decimal OEEMedio);

public record PagedResult<T>(List<T> Items, int TotalCount, int Page, int PageSize)
{
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
}
