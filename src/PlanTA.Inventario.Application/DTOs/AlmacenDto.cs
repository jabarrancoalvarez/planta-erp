namespace PlanTA.Inventario.Application.DTOs;

public record AlmacenDto(
    Guid Id,
    string Nombre,
    string? Direccion,
    string? Descripcion,
    bool EsPrincipal,
    List<UbicacionDto> Ubicaciones);

public record AlmacenListDto(
    Guid Id,
    string Nombre,
    string? Direccion,
    bool EsPrincipal,
    int TotalUbicaciones);

public record UbicacionDto(
    Guid Id,
    string Codigo,
    string? Nombre,
    int CapacidadMaxima,
    bool Activa);
