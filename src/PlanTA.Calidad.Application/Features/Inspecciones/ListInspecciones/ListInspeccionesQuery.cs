using PlanTA.Calidad.Application.DTOs;
using PlanTA.Calidad.Domain.Enums;
using PlanTA.SharedKernel.CQRS;

namespace PlanTA.Calidad.Application.Features.Inspecciones.ListInspecciones;

public record ListInspeccionesQuery(
    OrigenInspeccion? Origen = null,
    ResultadoInspeccion? Resultado = null,
    int Page = 1,
    int PageSize = 20) : IQuery<PagedResult<InspeccionListDto>>;
