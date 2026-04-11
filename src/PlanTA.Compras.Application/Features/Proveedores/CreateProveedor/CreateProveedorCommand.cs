using PlanTA.SharedKernel.CQRS;

namespace PlanTA.Compras.Application.Features.Proveedores.CreateProveedor;

public record CreateProveedorCommand(
    string RazonSocial,
    string NIF,
    string Email,
    int DiasVencimiento,
    decimal DescuentoProntoPago,
    string MetodoPago,
    string? Direccion = null,
    string? Ciudad = null,
    string? CodigoPostal = null,
    string? Pais = null,
    string? Telefono = null,
    string? Web = null) : ICommand<Guid>;
