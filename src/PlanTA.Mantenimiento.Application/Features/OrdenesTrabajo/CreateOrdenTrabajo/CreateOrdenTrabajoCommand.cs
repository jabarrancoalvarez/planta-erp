using PlanTA.Mantenimiento.Domain.Enums;
using PlanTA.SharedKernel.CQRS;

namespace PlanTA.Mantenimiento.Application.Features.OrdenesTrabajo.CreateOrdenTrabajo;

public record CreateOrdenTrabajoCommand(
    string Codigo,
    string Titulo,
    TipoMantenimiento Tipo,
    Guid ActivoId,
    string? Descripcion = null,
    PrioridadOT Prioridad = PrioridadOT.Media,
    DateTimeOffset? FechaPlanificada = null,
    decimal HorasEstimadas = 0,
    Guid? PlanMantenimientoId = null,
    Guid? IncidenciaId = null) : ICommand<Guid>;
