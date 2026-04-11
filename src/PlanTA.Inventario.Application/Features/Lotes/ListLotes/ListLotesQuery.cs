using PlanTA.Inventario.Application.DTOs;
using PlanTA.Inventario.Application.Features.Productos.ListProductos;
using PlanTA.SharedKernel.CQRS;

namespace PlanTA.Inventario.Application.Features.Lotes.ListLotes;

public record ListLotesQuery(
    Guid? ProductoId = null,
    int Page = 1,
    int PageSize = 20) : IQuery<PagedResult<LoteListDto>>;
