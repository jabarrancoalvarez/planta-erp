using PlanTA.Produccion.Application.DTOs;
using PlanTA.Produccion.Domain.Enums;
using PlanTA.SharedKernel.CQRS;

namespace PlanTA.Produccion.Application.Features.OrdenesFabricacion.ListOFs;

public record ListOFsQuery(
    string? Search = null,
    EstadoOF? Estado = null,
    int Page = 1,
    int PageSize = 20) : IQuery<PagedResult<OrdenFabricacionListDto>>;
