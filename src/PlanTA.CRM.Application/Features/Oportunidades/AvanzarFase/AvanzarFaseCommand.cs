using PlanTA.CRM.Domain.Enums;
using PlanTA.SharedKernel.CQRS;

namespace PlanTA.CRM.Application.Features.Oportunidades.AvanzarFase;

public record AvanzarFaseCommand(Guid Id, FaseOportunidad NuevaFase, int? ProbabilidadPct = null) : ICommand<bool>;
