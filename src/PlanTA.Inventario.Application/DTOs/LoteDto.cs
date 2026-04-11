using PlanTA.Inventario.Domain.Enums;

namespace PlanTA.Inventario.Application.DTOs;

public record LoteDto(
    Guid Id,
    string Codigo,
    Guid ProductoId,
    decimal CantidadInicial,
    decimal CantidadActual,
    EstadoLote Estado,
    DateTimeOffset? FechaCaducidad,
    DateTimeOffset FechaRecepcion,
    string? Origen,
    string? Notas);

public record LoteListDto(
    Guid Id,
    string Codigo,
    decimal CantidadActual,
    EstadoLote Estado,
    DateTimeOffset? FechaCaducidad);
