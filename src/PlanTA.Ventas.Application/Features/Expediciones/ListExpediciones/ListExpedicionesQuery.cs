using PlanTA.Ventas.Application.DTOs;
using PlanTA.Ventas.Domain.Enums;
using PlanTA.SharedKernel.CQRS;

namespace PlanTA.Ventas.Application.Features.Expediciones.ListExpediciones;

public record ListExpedicionesQuery(
    Guid? PedidoId = null,
    EstadoExpedicion? Estado = null,
    int Page = 1,
    int PageSize = 20) : IQuery<PagedResult<ExpedicionListDto>>;
