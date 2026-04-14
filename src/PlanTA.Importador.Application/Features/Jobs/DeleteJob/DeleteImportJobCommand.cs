using PlanTA.SharedKernel.CQRS;

namespace PlanTA.Importador.Application.Features.Jobs.DeleteJob;

public record DeleteImportJobCommand(Guid Id) : ICommand<bool>;
