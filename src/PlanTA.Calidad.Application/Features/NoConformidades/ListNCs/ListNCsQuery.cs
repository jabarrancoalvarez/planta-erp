using PlanTA.Calidad.Application.DTOs;
using PlanTA.Calidad.Domain.Enums;
using PlanTA.SharedKernel.CQRS;

namespace PlanTA.Calidad.Application.Features.NoConformidades.ListNCs;

public record ListNCsQuery(
    EstadoNoConformidad? Estado = null,
    SeveridadNC? Severidad = null,
    int Page = 1,
    int PageSize = 20) : IQuery<PagedResult<NCListDto>>;
