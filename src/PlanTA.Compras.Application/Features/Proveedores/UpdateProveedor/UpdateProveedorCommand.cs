using PlanTA.SharedKernel.CQRS;

namespace PlanTA.Compras.Application.Features.Proveedores.UpdateProveedor;

public record UpdateProveedorCommand(
    Guid ProveedorId,
    string RazonSocial,
    string Email,
    int DiasVencimiento,
    decimal DescuentoProntoPago,
    string MetodoPago,
    string? Direccion,
    string? Ciudad,
    string? CodigoPostal,
    string? Pais,
    string? Telefono,
    string? Web) : ICommand<Guid>;
