using PlanTA.SharedKernel.CQRS;

namespace PlanTA.RRHH.Application.Features.Ausencias.DeleteAusencia;

public record DeleteAusenciaCommand(Guid AusenciaId) : ICommand<bool>;
