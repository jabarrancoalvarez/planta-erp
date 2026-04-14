using PlanTA.SharedKernel.CQRS;

namespace PlanTA.CRM.Application.Features.Leads.DeleteLead;

public record DeleteLeadCommand(Guid LeadId) : ICommand<bool>;
