using PlanTA.Inventario.Application.DTOs;
using PlanTA.SharedKernel.CQRS;

namespace PlanTA.Inventario.Application.Features.Productos.ListProductos;

public record ListProductosQuery(
    string? Search = null,
    int Page = 1,
    int PageSize = 20) : IQuery<PagedResult<ProductoListDto>>;

public record PagedResult<T>(List<T> Items, int TotalCount, int Page, int PageSize)
{
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
}
