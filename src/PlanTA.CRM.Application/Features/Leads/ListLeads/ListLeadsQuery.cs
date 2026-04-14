using PlanTA.CRM.Application.DTOs;
using PlanTA.CRM.Domain.Enums;
using PlanTA.SharedKernel.CQRS;

namespace PlanTA.CRM.Application.Features.Leads.ListLeads;

public record ListLeadsQuery(
    string? Search = null, EstadoLead? Estado = null,
    int Page = 1, int PageSize = 20) : IQuery<PagedResult<LeadListDto>>;
