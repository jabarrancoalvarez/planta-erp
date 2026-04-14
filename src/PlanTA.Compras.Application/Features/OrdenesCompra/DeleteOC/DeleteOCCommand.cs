using PlanTA.SharedKernel.CQRS;

namespace PlanTA.Compras.Application.Features.OrdenesCompra.DeleteOC;

public record DeleteOCCommand(Guid OrdenCompraId) : ICommand<Guid>;
