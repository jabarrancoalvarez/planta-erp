using PlanTA.SharedKernel.CQRS;

namespace PlanTA.Facturacion.Application.Features.Facturas.DeleteFactura;

public record DeleteFacturaCommand(Guid Id) : ICommand<bool>;
