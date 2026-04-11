namespace PlanTA.Ventas.Application.DTOs;

public record ClienteDetailDto(
    Guid Id,
    string RazonSocial,
    string NIF,
    string? DireccionEnvio,
    string? DireccionFacturacion,
    string? Ciudad,
    string? CodigoPostal,
    string? Pais,
    string Email,
    string? Telefono,
    string? Web,
    bool Activo,
    DateTimeOffset CreatedAt,
    List<ContactoClienteDto> Contactos);

public record ClienteListDto(
    Guid Id,
    string RazonSocial,
    string NIF,
    string? Ciudad,
    string Email,
    bool Activo);

public record ContactoClienteDto(
    Guid Id,
    string Nombre,
    string? Cargo,
    string Email,
    string? Telefono,
    bool EsPrincipal);
