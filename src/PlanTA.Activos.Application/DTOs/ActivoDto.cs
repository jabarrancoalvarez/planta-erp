using PlanTA.Activos.Domain.Enums;

namespace PlanTA.Activos.Application.DTOs;

public record ActivoDto(
    Guid Id,
    string Codigo,
    string Nombre,
    string? Descripcion,
    TipoActivo Tipo,
    CriticidadActivo Criticidad,
    EstadoActivo Estado,
    Guid? ActivoPadreId,
    string? Ubicacion,
    string? Fabricante,
    string? Modelo,
    string? NumeroSerie,
    DateTimeOffset? FechaAdquisicion,
    decimal CosteAdquisicion,
    decimal HorasUso,
    DateTimeOffset CreatedAt);

public record ActivoListDto(
    Guid Id,
    string Codigo,
    string Nombre,
    TipoActivo Tipo,
    CriticidadActivo Criticidad,
    EstadoActivo Estado,
    string? Ubicacion);

public record PagedResult<T>(List<T> Items, int TotalCount, int Page, int PageSize)
{
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
}
