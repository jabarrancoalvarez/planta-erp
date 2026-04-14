using PlanTA.Costes.Application.DTOs;
using PlanTA.SharedKernel.CQRS;

namespace PlanTA.Costes.Application.Features.Imputaciones.ListImputaciones;

public record ListImputacionesQuery(
    Guid? OrdenFabricacionId = null,
    Guid? OrdenTrabajoId = null,
    int Page = 1,
    int PageSize = 50) : IQuery<PagedResult<ImputacionCosteDto>>;
