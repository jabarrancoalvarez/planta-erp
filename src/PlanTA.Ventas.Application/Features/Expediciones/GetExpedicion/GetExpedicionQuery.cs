using PlanTA.Ventas.Application.DTOs;
using PlanTA.SharedKernel.CQRS;

namespace PlanTA.Ventas.Application.Features.Expediciones.GetExpedicion;

public record GetExpedicionQuery(Guid ExpedicionId) : IQuery<ExpedicionDetailDto>;
