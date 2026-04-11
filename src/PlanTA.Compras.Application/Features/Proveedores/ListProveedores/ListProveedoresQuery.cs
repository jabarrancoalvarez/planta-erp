using PlanTA.Compras.Application.DTOs;
using PlanTA.SharedKernel.CQRS;

namespace PlanTA.Compras.Application.Features.Proveedores.ListProveedores;

public record ListProveedoresQuery(
    string? Search = null,
    int Page = 1,
    int PageSize = 20) : IQuery<PagedResult<ProveedorListDto>>;
