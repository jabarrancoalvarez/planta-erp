using PlanTA.Inventario.Application.DTOs;
using PlanTA.SharedKernel.CQRS;

namespace PlanTA.Inventario.Application.Features.Almacenes.ListAlmacenes;

public record ListAlmacenesQuery : IQuery<List<AlmacenListDto>>;
