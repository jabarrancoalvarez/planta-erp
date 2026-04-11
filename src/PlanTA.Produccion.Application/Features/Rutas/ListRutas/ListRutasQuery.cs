using PlanTA.Produccion.Application.DTOs;
using PlanTA.SharedKernel.CQRS;

namespace PlanTA.Produccion.Application.Features.Rutas.ListRutas;

public record ListRutasQuery(
    string? Search = null,
    int Page = 1,
    int PageSize = 20) : IQuery<PagedResult<RutaProduccionListDto>>;
