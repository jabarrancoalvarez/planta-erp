using PlanTA.SharedKernel.CQRS;

namespace PlanTA.OEE.Application.Features.Registros.DeleteRegistroOEE;

public record DeleteRegistroOEECommand(Guid Id) : ICommand<bool>;
