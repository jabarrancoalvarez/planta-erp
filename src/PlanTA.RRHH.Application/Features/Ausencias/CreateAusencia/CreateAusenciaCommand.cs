using PlanTA.RRHH.Domain.Enums;
using PlanTA.SharedKernel.CQRS;

namespace PlanTA.RRHH.Application.Features.Ausencias.CreateAusencia;

public record CreateAusenciaCommand(
    Guid EmpleadoId,
    TipoAusencia Tipo,
    DateTimeOffset FechaInicio,
    DateTimeOffset FechaFin,
    string? Motivo = null) : ICommand<Guid>;
