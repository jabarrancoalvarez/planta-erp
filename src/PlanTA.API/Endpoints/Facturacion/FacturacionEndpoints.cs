using MediatR;
using PlanTA.Facturacion.Application.Features.Facturas.CreateFactura;
using PlanTA.Facturacion.Application.Features.Facturas.EmitirFactura;
using PlanTA.Facturacion.Application.Features.Facturas.EnviarVerifactu;
using PlanTA.Facturacion.Application.Features.Facturas.GetFactura;
using PlanTA.Facturacion.Application.Features.Facturas.ListFacturas;
using PlanTA.Facturacion.Domain.Enums;
using PlanTA.SharedKernel.Extensions;

namespace PlanTA.API.Endpoints.Facturacion;

public sealed class FacturacionEndpoints : IEndpointGroup
{
    public string? RoutePrefix => "/api/v1/facturacion";

    public void MapEndpoints(RouteGroupBuilder group)
    {
        group.MapGet("/facturas", async (EstadoFactura? estado, Guid? clienteId, int? page, int? pageSize,
            IMediator m, CancellationToken ct) =>
        {
            var r = await m.Send(new ListFacturasQuery(estado, clienteId, page ?? 1, pageSize ?? 20), ct);
            return r.ToHttpResult();
        }).WithName("ListFacturas").WithTags("Facturacion");

        group.MapGet("/facturas/{id:guid}", async (Guid id, IMediator m, CancellationToken ct) =>
            (await m.Send(new GetFacturaQuery(id), ct)).ToHttpResult())
            .WithName("GetFactura").WithTags("Facturacion");

        group.MapPost("/facturas", async (CreateFacturaCommand cmd, IMediator m, CancellationToken ct) =>
            (await m.Send(cmd, ct)).ToHttpResult(201)).WithName("CreateFactura").WithTags("Facturacion");

        group.MapPost("/facturas/{id:guid}/emitir", async (Guid id, IMediator m, CancellationToken ct) =>
            (await m.Send(new EmitirFacturaCommand(id), ct)).ToHttpResult())
            .WithName("EmitirFactura").WithTags("Facturacion");

        group.MapPost("/facturas/{id:guid}/verifactu", async (Guid id, IMediator m, CancellationToken ct) =>
            (await m.Send(new EnviarVerifactuCommand(id), ct)).ToHttpResult())
            .WithName("EnviarVerifactu").WithTags("Facturacion");
    }
}
