namespace PlanTA.Produccion.Application.DTOs;

public record ListaMaterialesDetailDto(
    Guid Id,
    Guid ProductoId,
    string Nombre,
    string? Descripcion,
    int VersionBOM,
    bool Activo,
    DateTimeOffset CreatedAt,
    List<LineaBOMDto> Lineas);

public record ListaMaterialesListDto(
    Guid Id,
    Guid ProductoId,
    string Nombre,
    int VersionBOM,
    bool Activo,
    int NumeroLineas);

public record LineaBOMDto(
    Guid Id,
    Guid ComponenteProductoId,
    decimal Cantidad,
    string UnidadMedida,
    decimal Merma,
    int Orden);
