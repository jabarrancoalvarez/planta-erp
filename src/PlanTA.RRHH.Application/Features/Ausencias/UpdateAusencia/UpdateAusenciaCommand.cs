using PlanTA.RRHH.Domain.Enums;
using PlanTA.SharedKernel.CQRS;

namespace PlanTA.RRHH.Application.Features.Ausencias.UpdateAusencia;

public record UpdateAusenciaCommand(
    Guid AusenciaId,
    TipoAusencia Tipo,
    DateTimeOffset FechaInicio,
    DateTimeOffset FechaFin,
    string? Motivo) : ICommand<bool>;
