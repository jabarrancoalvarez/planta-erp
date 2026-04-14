using PlanTA.SharedKernel.CQRS;

namespace PlanTA.RRHH.Application.Features.Ausencias.AprobarAusencia;

public record AprobarAusenciaCommand(Guid Id, Guid UserId, bool Aprobar) : ICommand<bool>;
