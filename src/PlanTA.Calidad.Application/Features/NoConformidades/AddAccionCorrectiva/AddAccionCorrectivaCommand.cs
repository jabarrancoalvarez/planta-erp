using PlanTA.SharedKernel.CQRS;

namespace PlanTA.Calidad.Application.Features.NoConformidades.AddAccionCorrectiva;

public record AddAccionCorrectivaCommand(
    Guid NoConformidadId,
    string Descripcion,
    string? ResponsableUserId = null,
    DateTimeOffset? FechaLimite = null) : ICommand<Guid>;
