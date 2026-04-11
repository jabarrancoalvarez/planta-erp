using PlanTA.Compras.Application.DTOs;
using PlanTA.Compras.Domain.Enums;
using PlanTA.SharedKernel.CQRS;

namespace PlanTA.Compras.Application.Features.OrdenesCompra.ListOCs;

public record ListOCsQuery(
    string? Search = null,
    EstadoOC? Estado = null,
    int Page = 1,
    int PageSize = 20) : IQuery<PagedResult<OCListDto>>;
