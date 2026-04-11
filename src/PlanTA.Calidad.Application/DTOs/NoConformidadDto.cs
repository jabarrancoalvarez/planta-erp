using PlanTA.Calidad.Domain.Enums;

namespace PlanTA.Calidad.Application.DTOs;

public record NCDetailDto(
    Guid Id,
    string Codigo,
    Guid? InspeccionId,
    OrigenInspeccion OrigenInspeccion,
    Guid ReferenciaOrigenId,
    string Descripcion,
    SeveridadNC SeveridadNC,
    EstadoNoConformidad EstadoNoConformidad,
    string? CausaRaiz,
    DateTimeOffset FechaDeteccion,
    DateTimeOffset? FechaCierre,
    string? ResponsableUserId,
    DateTimeOffset CreatedAt,
    List<AccionCorrectivaDto> Acciones);

public record NCListDto(
    Guid Id,
    string Codigo,
    OrigenInspeccion OrigenInspeccion,
    SeveridadNC SeveridadNC,
    EstadoNoConformidad EstadoNoConformidad,
    DateTimeOffset FechaDeteccion);

public record AccionCorrectivaDto(
    Guid Id,
    string Descripcion,
    string? ResponsableUserId,
    DateTimeOffset? FechaLimite,
    DateTimeOffset? FechaCompletada,
    bool Completada);
