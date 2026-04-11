using PlanTA.SharedKernel.CQRS;

namespace PlanTA.Ventas.Application.Features.Clientes.UpdateCliente;

public record UpdateClienteCommand(
    Guid ClienteId,
    string RazonSocial,
    string Email,
    string? DireccionEnvio,
    string? DireccionFacturacion,
    string? Ciudad,
    string? CodigoPostal,
    string? Pais,
    string? Telefono,
    string? Web) : ICommand<Guid>;
