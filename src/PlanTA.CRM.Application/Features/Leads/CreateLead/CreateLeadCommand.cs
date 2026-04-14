using PlanTA.CRM.Domain.Enums;
using PlanTA.SharedKernel.CQRS;

namespace PlanTA.CRM.Application.Features.Leads.CreateLead;

public record CreateLeadCommand(
    string Nombre, OrigenLead Origen,
    string? Empresa = null, string? Email = null, string? Telefono = null,
    string? Notas = null, Guid? AsignadoAUserId = null) : ICommand<Guid>;
