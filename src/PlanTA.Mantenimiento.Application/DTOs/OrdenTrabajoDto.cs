using PlanTA.Mantenimiento.Domain.Enums;

namespace PlanTA.Mantenimiento.Application.DTOs;

public record OrdenTrabajoDto(
    Guid Id,
    string Codigo,
    string Titulo,
    string? Descripcion,
    TipoMantenimiento Tipo,
    EstadoOT Estado,
    PrioridadOT Prioridad,
    Guid ActivoId,
    Guid? AsignadoAUserId,
    DateTimeOffset? FechaPlanificada,
    DateTimeOffset? FechaInicio,
    DateTimeOffset? FechaFin,
    decimal HorasEstimadas,
    decimal HorasReales,
    decimal CosteManoObra,
    decimal CosteRepuestos,
    string? NotasCierre);

public record OrdenTrabajoListDto(
    Guid Id,
    string Codigo,
    string Titulo,
    TipoMantenimiento Tipo,
    EstadoOT Estado,
    PrioridadOT Prioridad,
    Guid ActivoId,
    DateTimeOffset? FechaPlanificada);

public record PlanMantenimientoDto(
    Guid Id,
    string Codigo,
    string Nombre,
    Guid ActivoId,
    TipoMantenimiento Tipo,
    FrecuenciaPlan Frecuencia,
    int Intervalo,
    DateTimeOffset? ProximaEjecucion,
    bool Activo);

public record PagedResult<T>(List<T> Items, int TotalCount, int Page, int PageSize)
{
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
}
