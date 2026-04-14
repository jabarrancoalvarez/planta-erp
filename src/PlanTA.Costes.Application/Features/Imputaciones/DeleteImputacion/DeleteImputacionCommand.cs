using PlanTA.SharedKernel.CQRS;

namespace PlanTA.Costes.Application.Features.Imputaciones.DeleteImputacion;

public record DeleteImputacionCommand(Guid ImputacionId) : ICommand<bool>;
