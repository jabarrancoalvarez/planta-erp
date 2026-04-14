using PlanTA.Activos.Application.DTOs;
using PlanTA.Activos.Domain.Enums;
using PlanTA.SharedKernel.CQRS;

namespace PlanTA.Activos.Application.Features.Activos.ListActivos;

public record ListActivosQuery(
    string? Search = null,
    TipoActivo? Tipo = null,
    EstadoActivo? Estado = null,
    CriticidadActivo? Criticidad = null,
    int Page = 1,
    int PageSize = 20) : IQuery<PagedResult<ActivoListDto>>;
