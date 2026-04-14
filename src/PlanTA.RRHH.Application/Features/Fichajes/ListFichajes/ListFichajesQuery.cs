using PlanTA.RRHH.Application.DTOs;
using PlanTA.SharedKernel.CQRS;

namespace PlanTA.RRHH.Application.Features.Fichajes.ListFichajes;

public record ListFichajesQuery(
    Guid? EmpleadoId = null,
    DateTimeOffset? Desde = null,
    DateTimeOffset? Hasta = null,
    int Page = 1,
    int PageSize = 50) : IQuery<PagedResult<FichajeDto>>;
