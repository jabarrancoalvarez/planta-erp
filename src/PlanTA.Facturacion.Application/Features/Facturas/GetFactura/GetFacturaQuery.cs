using PlanTA.Facturacion.Application.DTOs;
using PlanTA.SharedKernel.CQRS;

namespace PlanTA.Facturacion.Application.Features.Facturas.GetFactura;

public record GetFacturaQuery(Guid Id) : IQuery<FacturaDto>;
