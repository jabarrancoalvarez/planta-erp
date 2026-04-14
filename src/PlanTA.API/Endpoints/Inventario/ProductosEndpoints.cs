using MediatR;
using PlanTA.Inventario.Application.Features.Productos.CreateProducto;
using PlanTA.Inventario.Application.Features.Productos.DeleteProducto;
using PlanTA.Inventario.Application.Features.Productos.GetProducto;
using PlanTA.Inventario.Application.Features.Productos.ListProductos;
using PlanTA.Inventario.Application.Features.Productos.UpdateProducto;
using PlanTA.SharedKernel.Extensions;

namespace PlanTA.API.Endpoints.Inventario;

public sealed class ProductosEndpoints : IEndpointGroup
{
    public string? RoutePrefix => "/api/v1/inventario/productos";

    public void MapEndpoints(RouteGroupBuilder group)
    {
        group.MapGet("/", async (string? search, int? page, int? pageSize, IMediator m, CancellationToken ct) =>
        {
            var result = await m.Send(new ListProductosQuery(search, page ?? 1, pageSize ?? 20), ct);
            return result.ToHttpResult();
        })
        .WithName("ListProductos")
        .WithTags("Productos");

        group.MapGet("/{id:guid}", async (Guid id, IMediator m, CancellationToken ct) =>
        {
            var result = await m.Send(new GetProductoQuery(id), ct);
            return result.ToHttpResult();
        })
        .WithName("GetProducto")
        .WithTags("Productos");

        group.MapPost("/", async (CreateProductoCommand command, IMediator m, CancellationToken ct) =>
        {
            var result = await m.Send(command, ct);
            return result.ToHttpResult(201);
        })
        .WithName("CreateProducto")
        .WithTags("Productos");

        group.MapPut("/{id:guid}", async (Guid id, UpdateProductoRequest req, IMediator m, CancellationToken ct) =>
        {
            var result = await m.Send(new UpdateProductoCommand(
                id, req.Nombre, req.Descripcion, req.PrecioCompra, req.PrecioVenta, req.CategoriaId), ct);
            return result.ToHttpResult();
        })
        .WithName("UpdateProducto")
        .WithTags("Productos");

        group.MapDelete("/{id:guid}", async (Guid id, IMediator m, CancellationToken ct) =>
        {
            var result = await m.Send(new DeleteProductoCommand(id), ct);
            return result.ToHttpResult();
        })
        .WithName("DeleteProducto")
        .WithTags("Productos");
    }
}

public record UpdateProductoRequest(
    string Nombre,
    string? Descripcion,
    decimal PrecioCompra,
    decimal PrecioVenta,
    Guid? CategoriaId);
