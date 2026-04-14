using PlanTA.OEE.Application.DTOs;
using PlanTA.SharedKernel.CQRS;

namespace PlanTA.OEE.Application.Features.Registros.ResumenActivo;

public record ResumenActivoQuery(
    Guid ActivoId,
    DateTimeOffset? Desde = null,
    DateTimeOffset? Hasta = null) : IQuery<ResumenOEEPorActivoDto>;
