using PlanTA.SharedKernel.CQRS;

namespace PlanTA.CRM.Application.Features.Oportunidades.UpdateOportunidad;

public record UpdateOportunidadCommand(
    Guid OportunidadId,
    string Titulo,
    decimal ImporteEstimado,
    DateTimeOffset? FechaCierreEstimada,
    string? Descripcion) : ICommand<bool>;
