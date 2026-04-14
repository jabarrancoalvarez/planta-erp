using PlanTA.RRHH.Application.DTOs;
using PlanTA.RRHH.Domain.Enums;
using PlanTA.SharedKernel.CQRS;

namespace PlanTA.RRHH.Application.Features.Ausencias.ListAusencias;

public record ListAusenciasQuery(
    Guid? EmpleadoId = null,
    EstadoAusencia? Estado = null,
    int Page = 1,
    int PageSize = 20) : IQuery<PagedResult<AusenciaDto>>;
