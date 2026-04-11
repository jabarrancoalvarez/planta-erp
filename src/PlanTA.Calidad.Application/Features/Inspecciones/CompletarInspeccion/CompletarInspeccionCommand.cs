using PlanTA.Calidad.Domain.Enums;
using PlanTA.SharedKernel.CQRS;

namespace PlanTA.Calidad.Application.Features.Inspecciones.CompletarInspeccion;

public record CompletarInspeccionCommand(Guid InspeccionId) : ICommand<ResultadoInspeccion>;
