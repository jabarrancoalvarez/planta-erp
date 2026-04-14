using PlanTA.Incidencias.Domain.Enums;

namespace PlanTA.Incidencias.Application.DTOs;

public record IncidenciaDto(
    Guid Id,
    string Codigo,
    string Titulo,
    string Descripcion,
    TipoIncidencia Tipo,
    SeveridadIncidencia Severidad,
    EstadoIncidencia Estado,
    Guid? ActivoId,
    string? UbicacionTexto,
    Guid ReportadoPorUserId,
    Guid? AsignadoAUserId,
    DateTimeOffset FechaApertura,
    DateTimeOffset? FechaCierre,
    Guid? OrdenTrabajoId,
    string? CausaRaiz,
    string? ResolucionNotas,
    bool ParadaLinea);

public record IncidenciaListDto(
    Guid Id,
    string Codigo,
    string Titulo,
    TipoIncidencia Tipo,
    SeveridadIncidencia Severidad,
    EstadoIncidencia Estado,
    Guid? ActivoId,
    DateTimeOffset FechaApertura,
    bool ParadaLinea);

public record PagedResult<T>(List<T> Items, int TotalCount, int Page, int PageSize)
{
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
}
