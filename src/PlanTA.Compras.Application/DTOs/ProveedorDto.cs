namespace PlanTA.Compras.Application.DTOs;

public record ProveedorDetailDto(
    Guid Id,
    string RazonSocial,
    string NIF,
    string? Direccion,
    string? Ciudad,
    string? CodigoPostal,
    string? Pais,
    string Email,
    string? Telefono,
    string? Web,
    int DiasVencimiento,
    decimal DescuentoProntoPago,
    string MetodoPago,
    bool Activo,
    DateTimeOffset CreatedAt,
    List<ContactoProveedorDto> Contactos);

public record ProveedorListDto(
    Guid Id,
    string RazonSocial,
    string NIF,
    string? Ciudad,
    string Email,
    bool Activo);

public record ContactoProveedorDto(
    Guid Id,
    string Nombre,
    string? Cargo,
    string Email,
    string? Telefono,
    bool EsPrincipal);
