using PlanTA.Produccion.Application.DTOs;
using PlanTA.SharedKernel.CQRS;

namespace PlanTA.Produccion.Application.Features.BOM.ListBOMs;

public record ListBOMsQuery(
    string? Search = null,
    int Page = 1,
    int PageSize = 20) : IQuery<PagedResult<ListaMaterialesListDto>>;
