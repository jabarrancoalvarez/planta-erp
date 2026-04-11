using PlanTA.Produccion.Domain.Enums;

namespace PlanTA.Produccion.Application.DTOs;

public record OrdenFabricacionDetailDto(
    Guid Id,
    string CodigoOF,
    Guid ProductoId,
    Guid ListaMaterialesId,
    Guid? RutaProduccionId,
    decimal CantidadPlanificada,
    string UnidadMedida,
    EstadoOF EstadoOF,
    DateTimeOffset? FechaInicio,
    DateTimeOffset? FechaFin,
    int Prioridad,
    string? Observaciones,
    DateTimeOffset CreatedAt,
    List<LineaConsumoOFDto> LineasConsumo,
    List<ParteProduccionDto> PartesProduccion);

public record OrdenFabricacionListDto(
    Guid Id,
    string CodigoOF,
    Guid ProductoId,
    decimal CantidadPlanificada,
    string UnidadMedida,
    EstadoOF EstadoOF,
    DateTimeOffset? FechaInicio,
    int Prioridad);

public record LineaConsumoOFDto(
    Guid Id,
    Guid ProductoId,
    Guid? LoteId,
    decimal Cantidad,
    DateTimeOffset FechaConsumo);

public record ParteProduccionDto(
    Guid Id,
    DateTimeOffset FechaRegistro,
    decimal UnidadesBuenas,
    decimal UnidadesDefectuosas,
    decimal Merma,
    Guid? LoteGeneradoId,
    string? Observaciones);
