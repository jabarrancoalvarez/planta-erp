using PlanTA.SharedKernel.CQRS;

namespace PlanTA.CRM.Application.Features.Leads.UpdateLead;

public record UpdateLeadCommand(
    Guid LeadId,
    string Nombre,
    string? Empresa,
    string? Email,
    string? Telefono,
    string? Notas) : ICommand<bool>;
