using PlanTA.Costes.Application.DTOs;
using PlanTA.SharedKernel.CQRS;

namespace PlanTA.Costes.Application.Features.Imputaciones.ResumenOF;

public record ResumenOFQuery(Guid OrdenFabricacionId) : IQuery<ResumenCosteOFDto>;
