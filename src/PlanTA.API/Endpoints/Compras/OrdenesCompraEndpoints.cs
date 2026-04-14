using MediatR;
using PlanTA.Compras.Application.Features.OrdenesCompra.AddLineaOC;
using PlanTA.Compras.Application.Features.OrdenesCompra.CambiarEstadoOC;
using PlanTA.Compras.Application.Features.OrdenesCompra.CreateOC;
using PlanTA.Compras.Application.Features.OrdenesCompra.DeleteOC;
using PlanTA.Compras.Application.Features.OrdenesCompra.GetOC;
using PlanTA.Compras.Application.Features.OrdenesCompra.ListOCs;
using PlanTA.Compras.Application.Features.OrdenesCompra.UpdateOC;
using PlanTA.Compras.Domain.Enums;
using PlanTA.SharedKernel.Extensions;

namespace PlanTA.API.Endpoints.Compras;

public sealed class OrdenesCompraEndpoints : IEndpointGroup
{
    public string? RoutePrefix => "/api/v1/compras/ordenes";

    public void MapEndpoints(RouteGroupBuilder group)
    {
        group.MapGet("/", async (string? search, EstadoOC? estado, int? page, int? pageSize, IMediator m, CancellationToken ct) =>
        {
            var result = await m.Send(new ListOCsQuery(search, estado, page ?? 1, pageSize ?? 20), ct);
            return result.ToHttpResult();
        })
        .WithName("ListOrdenesCompra")
        .WithTags("OrdenesCompra");

        group.MapGet("/{id:guid}", async (Guid id, IMediator m, CancellationToken ct) =>
        {
            var result = await m.Send(new GetOCQuery(id), ct);
            return result.ToHttpResult();
        })
        .WithName("GetOrdenCompra")
        .WithTags("OrdenesCompra");

        group.MapPost("/", async (CreateOCCommand command, IMediator m, CancellationToken ct) =>
        {
            var result = await m.Send(command, ct);
            return result.ToHttpResult(201);
        })
        .WithName("CreateOrdenCompra")
        .WithTags("OrdenesCompra");

        group.MapPost("/{id:guid}/lineas", async (Guid id, AddLineaOCRequest req, IMediator m, CancellationToken ct) =>
        {
            var result = await m.Send(new AddLineaOCCommand(
                id, req.ProductoId, req.Descripcion, req.Cantidad, req.PrecioUnitario), ct);
            return result.ToHttpResult(201);
        })
        .WithName("AddLineaOC")
        .WithTags("OrdenesCompra");

        group.MapPut("/{id:guid}", async (Guid id, UpdateOCRequest req, IMediator m, CancellationToken ct) =>
        {
            var result = await m.Send(new UpdateOCCommand(id, req.FechaEntregaEstimada, req.Observaciones), ct);
            return result.ToHttpResult();
        })
        .WithName("UpdateOrdenCompra")
        .WithTags("OrdenesCompra");

        group.MapPut("/{id:guid}/estado", async (Guid id, CambiarEstadoOCRequest req, IMediator m, CancellationToken ct) =>
        {
            var result = await m.Send(new CambiarEstadoOCCommand(id, req.EstadoDestino, req.Motivo), ct);
            return result.ToHttpResult();
        })
        .WithName("CambiarEstadoOC")
        .WithTags("OrdenesCompra");

        group.MapDelete("/{id:guid}", async (Guid id, IMediator m, CancellationToken ct) =>
        {
            var result = await m.Send(new DeleteOCCommand(id), ct);
            return result.ToHttpResult();
        })
        .WithName("DeleteOrdenCompra")
        .WithTags("OrdenesCompra");
    }
}

public record AddLineaOCRequest(
    Guid ProductoId,
    string Descripcion,
    decimal Cantidad,
    decimal PrecioUnitario);

public record UpdateOCRequest(DateTimeOffset? FechaEntregaEstimada, string? Observaciones);

public record CambiarEstadoOCRequest(EstadoOC EstadoDestino, string? Motivo = null);
