using PlanTA.SharedKernel.CQRS;

namespace PlanTA.RRHH.Application.Features.Fichajes.DeleteFichaje;

public record DeleteFichajeCommand(Guid FichajeId) : ICommand<bool>;
