using PlanTA.Calidad.Domain.Enums;

namespace PlanTA.Calidad.Application.DTOs;

public record PlantillaDetailDto(
    Guid Id,
    string Nombre,
    string? Descripcion,
    OrigenInspeccion OrigenInspeccion,
    int Version,
    bool Activa,
    DateTimeOffset CreatedAt,
    List<CriterioDto> Criterios);

public record PlantillaListDto(
    Guid Id,
    string Nombre,
    OrigenInspeccion OrigenInspeccion,
    int Version,
    bool Activa,
    int CantidadCriterios);

public record CriterioDto(
    Guid Id,
    string Nombre,
    string? Descripcion,
    string TipoMedida,
    decimal? ValorMinimo,
    decimal? ValorMaximo,
    string? UnidadMedida,
    bool EsObligatorio,
    int Orden);
