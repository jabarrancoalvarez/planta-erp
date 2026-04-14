using PlanTA.Facturacion.Application.DTOs;
using PlanTA.Facturacion.Domain.Enums;
using PlanTA.SharedKernel.CQRS;

namespace PlanTA.Facturacion.Application.Features.Facturas.ListFacturas;

public record ListFacturasQuery(
    EstadoFactura? Estado = null,
    Guid? ClienteId = null,
    int Page = 1,
    int PageSize = 20) : IQuery<PagedResult<FacturaListDto>>;
