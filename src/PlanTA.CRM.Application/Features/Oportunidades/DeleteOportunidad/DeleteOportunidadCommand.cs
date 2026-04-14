using PlanTA.SharedKernel.CQRS;

namespace PlanTA.CRM.Application.Features.Oportunidades.DeleteOportunidad;

public record DeleteOportunidadCommand(Guid OportunidadId) : ICommand<bool>;
