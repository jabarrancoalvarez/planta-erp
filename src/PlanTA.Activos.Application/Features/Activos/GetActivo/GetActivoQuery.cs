using PlanTA.Activos.Application.DTOs;
using PlanTA.SharedKernel.CQRS;

namespace PlanTA.Activos.Application.Features.Activos.GetActivo;

public record GetActivoQuery(Guid Id) : IQuery<ActivoDto>;
