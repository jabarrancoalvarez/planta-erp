using PlanTA.RRHH.Domain.Enums;

namespace PlanTA.RRHH.Application.DTOs;

public record EmpleadoListDto(
    Guid Id, string Codigo, string Nombre, string Apellidos, string DNI,
    string Puesto, string? Departamento, bool Activo,
    string? Email, string? Telefono, decimal CosteHoraEstandar, int DiasVacacionesAnuales);

public record EmpleadoDto(
    Guid Id, string Codigo, string Nombre, string Apellidos, string DNI,
    string? Email, string? Telefono, string Puesto, string? Departamento,
    DateTimeOffset FechaAlta, DateTimeOffset? FechaBaja,
    decimal CosteHoraEstandar, int DiasVacacionesAnuales, Guid? UserId);

public record FichajeDto(
    Guid Id, Guid EmpleadoId, string EmpleadoNombre, TipoFichaje Tipo,
    DateTimeOffset Momento, Guid? ActivoId, Guid? OrdenFabricacionId,
    Guid? OrdenTrabajoId, string? Notas);

public record AusenciaDto(
    Guid Id, Guid EmpleadoId, string EmpleadoNombre, TipoAusencia Tipo,
    EstadoAusencia Estado, DateTimeOffset FechaInicio, DateTimeOffset FechaFin,
    int DiasTotales, string? Motivo);

public record PagedResult<T>(List<T> Items, int TotalCount, int Page, int PageSize)
{
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
}
