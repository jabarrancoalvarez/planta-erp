using MediatR;
using PlanTA.Ventas.Application.Features.Pedidos.AddLineaPedido;
using PlanTA.Ventas.Application.Features.Pedidos.CambiarEstadoPedido;
using PlanTA.Ventas.Application.Features.Pedidos.CreatePedido;
using PlanTA.Ventas.Application.Features.Pedidos.GetPedido;
using PlanTA.Ventas.Application.Features.Pedidos.ListPedidos;
using PlanTA.Ventas.Domain.Enums;
using PlanTA.SharedKernel.Extensions;

namespace PlanTA.API.Endpoints.Ventas;

public sealed class PedidosEndpoints : IEndpointGroup
{
    public string? RoutePrefix => "/api/v1/ventas/pedidos";

    public void MapEndpoints(RouteGroupBuilder group)
    {
        group.MapGet("/", async (string? search, EstadoPedido? estado, int? page, int? pageSize, IMediator m, CancellationToken ct) =>
        {
            var result = await m.Send(new ListPedidosQuery(search, estado, page ?? 1, pageSize ?? 20), ct);
            return result.ToHttpResult();
        })
        .WithName("ListPedidos")
        .WithTags("Pedidos");

        group.MapGet("/{id:guid}", async (Guid id, IMediator m, CancellationToken ct) =>
        {
            var result = await m.Send(new GetPedidoQuery(id), ct);
            return result.ToHttpResult();
        })
        .WithName("GetPedido")
        .WithTags("Pedidos");

        group.MapPost("/", async (CreatePedidoCommand command, IMediator m, CancellationToken ct) =>
        {
            var result = await m.Send(command, ct);
            return result.ToHttpResult(201);
        })
        .WithName("CreatePedido")
        .WithTags("Pedidos");

        group.MapPost("/{id:guid}/lineas", async (Guid id, AddLineaPedidoRequest req, IMediator m, CancellationToken ct) =>
        {
            var result = await m.Send(new AddLineaPedidoCommand(
                id, req.ProductoId, req.Descripcion, req.Cantidad, req.PrecioUnitario, req.Descuento), ct);
            return result.ToHttpResult(201);
        })
        .WithName("AddLineaPedido")
        .WithTags("Pedidos");

        group.MapPut("/{id:guid}/estado", async (Guid id, CambiarEstadoPedidoRequest req, IMediator m, CancellationToken ct) =>
        {
            var result = await m.Send(new CambiarEstadoPedidoCommand(id, req.EstadoDestino, req.Motivo), ct);
            return result.ToHttpResult();
        })
        .WithName("CambiarEstadoPedido")
        .WithTags("Pedidos");
    }
}

public record AddLineaPedidoRequest(
    Guid ProductoId,
    string Descripcion,
    decimal Cantidad,
    decimal PrecioUnitario,
    decimal Descuento = 0);

public record CambiarEstadoPedidoRequest(EstadoPedido EstadoDestino, string? Motivo = null);
