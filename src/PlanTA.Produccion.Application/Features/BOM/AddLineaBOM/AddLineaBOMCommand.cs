using PlanTA.SharedKernel.CQRS;

namespace PlanTA.Produccion.Application.Features.BOM.AddLineaBOM;

public record AddLineaBOMCommand(
    Guid ListaMaterialesId,
    Guid ComponenteProductoId,
    decimal Cantidad,
    string UnidadMedida,
    decimal Merma = 0,
    int? Orden = null) : ICommand<Guid>;
