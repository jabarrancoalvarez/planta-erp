using PlanTA.Inventario.Application.DTOs;
using PlanTA.SharedKernel.CQRS;

namespace PlanTA.Inventario.Application.Features.Alertas.ListAlertas;

public record ListAlertasQuery(Guid? ProductoId = null) : IQuery<List<AlertaStockDto>>;
