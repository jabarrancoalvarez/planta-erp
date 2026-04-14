using PlanTA.SharedKernel.CQRS;

namespace PlanTA.Calidad.Application.Features.NoConformidades.DeleteNC;

public record DeleteNCCommand(Guid NoConformidadId) : ICommand<Guid>;
