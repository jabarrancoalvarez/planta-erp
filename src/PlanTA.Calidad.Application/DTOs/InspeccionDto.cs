using PlanTA.Calidad.Domain.Enums;

namespace PlanTA.Calidad.Application.DTOs;

public record InspeccionDetailDto(
    Guid Id,
    Guid PlantillaInspeccionId,
    OrigenInspeccion OrigenInspeccion,
    Guid ReferenciaOrigenId,
    Guid? LoteId,
    DateTimeOffset FechaInspeccion,
    string? InspectorUserId,
    ResultadoInspeccion? ResultadoInspeccion,
    string? Observaciones,
    DateTimeOffset CreatedAt,
    List<ResultadoCriterioDto> Resultados);

public record InspeccionListDto(
    Guid Id,
    OrigenInspeccion OrigenInspeccion,
    Guid ReferenciaOrigenId,
    DateTimeOffset FechaInspeccion,
    ResultadoInspeccion? ResultadoInspeccion);

public record ResultadoCriterioDto(
    Guid Id,
    Guid CriterioInspeccionId,
    string NombreCriterio,
    bool EsObligatorio,
    string? ValorMedido,
    bool Cumple,
    string? Observaciones);
