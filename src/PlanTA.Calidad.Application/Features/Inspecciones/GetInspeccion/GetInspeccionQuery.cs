using PlanTA.Calidad.Application.DTOs;
using PlanTA.SharedKernel.CQRS;

namespace PlanTA.Calidad.Application.Features.Inspecciones.GetInspeccion;

public record GetInspeccionQuery(Guid InspeccionId) : IQuery<InspeccionDetailDto>;
