using PlanTA.SharedKernel.CQRS;

namespace PlanTA.CRM.Application.Features.Oportunidades.CreateOportunidad;

public record CreateOportunidadCommand(
    string Titulo, decimal ImporteEstimado,
    Guid? ClienteId = null, Guid? LeadId = null,
    DateTimeOffset? FechaCierreEstimada = null, string? Descripcion = null,
    Guid? PropietarioUserId = null) : ICommand<Guid>;
