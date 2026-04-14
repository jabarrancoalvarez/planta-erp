using PlanTA.OEE.Application.DTOs;
using PlanTA.SharedKernel.CQRS;

namespace PlanTA.OEE.Application.Features.Registros.ListRegistros;

public record ListRegistrosQuery(
    Guid? ActivoId = null,
    DateTimeOffset? Desde = null,
    DateTimeOffset? Hasta = null,
    int Page = 1,
    int PageSize = 50) : IQuery<PagedResult<RegistroOEEDto>>;
