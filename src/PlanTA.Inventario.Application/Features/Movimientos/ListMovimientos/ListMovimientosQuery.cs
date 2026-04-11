using PlanTA.Inventario.Application.DTOs;
using PlanTA.Inventario.Application.Features.Productos.ListProductos;
using PlanTA.SharedKernel.CQRS;

namespace PlanTA.Inventario.Application.Features.Movimientos.ListMovimientos;

public record ListMovimientosQuery(
    Guid? ProductoId = null,
    Guid? AlmacenId = null,
    int Page = 1,
    int PageSize = 20) : IQuery<PagedResult<MovimientoStockDto>>;
