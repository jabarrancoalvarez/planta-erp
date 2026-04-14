using PlanTA.CRM.Application.DTOs;
using PlanTA.CRM.Domain.Enums;
using PlanTA.SharedKernel.CQRS;

namespace PlanTA.CRM.Application.Features.Oportunidades.ListOportunidades;

public record ListOportunidadesQuery(
    FaseOportunidad? Fase = null,
    int Page = 1, int PageSize = 20) : IQuery<PagedResult<OportunidadListDto>>;
