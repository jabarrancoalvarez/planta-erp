using PlanTA.Compras.Application.DTOs;
using PlanTA.SharedKernel.CQRS;

namespace PlanTA.Compras.Application.Features.OrdenesCompra.GetOC;

public record GetOCQuery(Guid OrdenCompraId) : IQuery<OCDetailDto>;
