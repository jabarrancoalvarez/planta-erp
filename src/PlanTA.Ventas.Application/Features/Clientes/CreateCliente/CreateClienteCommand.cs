using PlanTA.SharedKernel.CQRS;

namespace PlanTA.Ventas.Application.Features.Clientes.CreateCliente;

public record CreateClienteCommand(
    string RazonSocial,
    string NIF,
    string Email,
    string? DireccionEnvio = null,
    string? DireccionFacturacion = null,
    string? Ciudad = null,
    string? CodigoPostal = null,
    string? Pais = null,
    string? Telefono = null,
    string? Web = null) : ICommand<Guid>;
