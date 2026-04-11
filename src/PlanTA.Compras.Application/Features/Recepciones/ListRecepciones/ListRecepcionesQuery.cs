using PlanTA.Compras.Application.DTOs;
using PlanTA.Compras.Domain.Enums;
using PlanTA.SharedKernel.CQRS;

namespace PlanTA.Compras.Application.Features.Recepciones.ListRecepciones;

public record ListRecepcionesQuery(
    Guid? OrdenCompraId = null,
    EstadoRecepcion? Estado = null,
    int Page = 1,
    int PageSize = 20) : IQuery<PagedResult<RecepcionListDto>>;
