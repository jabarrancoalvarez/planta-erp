using PlanTA.Produccion.Application.DTOs;
using PlanTA.SharedKernel.CQRS;

namespace PlanTA.Produccion.Application.Features.OrdenesFabricacion.GetOF;

public record GetOFQuery(Guid OrdenFabricacionId) : IQuery<OrdenFabricacionDetailDto>;
