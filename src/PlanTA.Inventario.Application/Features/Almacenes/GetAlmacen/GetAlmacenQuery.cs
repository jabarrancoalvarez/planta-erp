using PlanTA.Inventario.Application.DTOs;
using PlanTA.SharedKernel.CQRS;

namespace PlanTA.Inventario.Application.Features.Almacenes.GetAlmacen;

public record GetAlmacenQuery(Guid AlmacenId) : IQuery<AlmacenDto>;
