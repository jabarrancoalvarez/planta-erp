using PlanTA.Calidad.Application.DTOs;
using PlanTA.SharedKernel.CQRS;

namespace PlanTA.Calidad.Application.Features.NoConformidades.GetNC;

public record GetNCQuery(Guid NoConformidadId) : IQuery<NCDetailDto>;
