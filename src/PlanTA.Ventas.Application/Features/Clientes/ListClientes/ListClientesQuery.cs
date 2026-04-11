using PlanTA.Ventas.Application.DTOs;
using PlanTA.SharedKernel.CQRS;

namespace PlanTA.Ventas.Application.Features.Clientes.ListClientes;

public record ListClientesQuery(
    string? Search = null,
    int Page = 1,
    int PageSize = 20) : IQuery<PagedResult<ClienteListDto>>;
