using PlanTA.Ventas.Application.DTOs;
using PlanTA.SharedKernel.CQRS;

namespace PlanTA.Ventas.Application.Features.Clientes.GetCliente;

public record GetClienteQuery(Guid ClienteId) : IQuery<ClienteDetailDto>;
