using PlanTA.Produccion.Domain.Enums;

namespace PlanTA.Produccion.Application.DTOs;

public record RutaProduccionDetailDto(
    Guid Id,
    Guid ProductoId,
    string Nombre,
    string? Descripcion,
    bool Activa,
    DateTimeOffset CreatedAt,
    List<OperacionRutaDto> Operaciones);

public record RutaProduccionListDto(
    Guid Id,
    Guid ProductoId,
    string Nombre,
    bool Activa,
    int NumeroOperaciones);

public record OperacionRutaDto(
    Guid Id,
    int Numero,
    string Nombre,
    TipoOperacion TipoOperacion,
    decimal TiempoEstimadoMinutos,
    string CentroTrabajo,
    string? Instrucciones);
