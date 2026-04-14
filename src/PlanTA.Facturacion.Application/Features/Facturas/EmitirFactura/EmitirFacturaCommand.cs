using PlanTA.SharedKernel.CQRS;

namespace PlanTA.Facturacion.Application.Features.Facturas.EmitirFactura;

public record EmitirFacturaCommand(Guid Id) : ICommand<bool>;
