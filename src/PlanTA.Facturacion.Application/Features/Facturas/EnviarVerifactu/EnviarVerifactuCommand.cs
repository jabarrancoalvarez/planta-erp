using PlanTA.SharedKernel.CQRS;

namespace PlanTA.Facturacion.Application.Features.Facturas.EnviarVerifactu;

public record EnviarVerifactuCommand(Guid Id) : ICommand<string>;
