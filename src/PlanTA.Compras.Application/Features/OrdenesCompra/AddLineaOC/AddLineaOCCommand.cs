using PlanTA.SharedKernel.CQRS;

namespace PlanTA.Compras.Application.Features.OrdenesCompra.AddLineaOC;

public record AddLineaOCCommand(
    Guid OrdenCompraId,
    Guid ProductoId,
    string Descripcion,
    decimal Cantidad,
    decimal PrecioUnitario) : ICommand<Guid>;
