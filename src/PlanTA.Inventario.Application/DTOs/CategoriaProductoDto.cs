namespace PlanTA.Inventario.Application.DTOs;

public record CategoriaProductoDto(
    Guid Id,
    string Nombre,
    string? Descripcion,
    Guid? CategoriaPadreId);
