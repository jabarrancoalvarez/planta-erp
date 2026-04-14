using MediatR;
using PlanTA.Compras.Application.Features.Proveedores.CreateProveedor;
using PlanTA.Compras.Application.Features.Proveedores.DeleteProveedor;
using PlanTA.Compras.Application.Features.Proveedores.GetProveedor;
using PlanTA.Compras.Application.Features.Proveedores.ListProveedores;
using PlanTA.Compras.Application.Features.Proveedores.UpdateProveedor;
using PlanTA.SharedKernel.Extensions;

namespace PlanTA.API.Endpoints.Compras;

public sealed class ProveedoresEndpoints : IEndpointGroup
{
    public string? RoutePrefix => "/api/v1/compras/proveedores";

    public void MapEndpoints(RouteGroupBuilder group)
    {
        group.MapGet("/", async (string? search, int? page, int? pageSize, IMediator m, CancellationToken ct) =>
        {
            var result = await m.Send(new ListProveedoresQuery(search, page ?? 1, pageSize ?? 20), ct);
            return result.ToHttpResult();
        })
        .WithName("ListProveedores")
        .WithTags("Proveedores");

        group.MapGet("/{id:guid}", async (Guid id, IMediator m, CancellationToken ct) =>
        {
            var result = await m.Send(new GetProveedorQuery(id), ct);
            return result.ToHttpResult();
        })
        .WithName("GetProveedor")
        .WithTags("Proveedores");

        group.MapPost("/", async (CreateProveedorCommand command, IMediator m, CancellationToken ct) =>
        {
            var result = await m.Send(command, ct);
            return result.ToHttpResult(201);
        })
        .WithName("CreateProveedor")
        .WithTags("Proveedores");

        group.MapPut("/{id:guid}", async (Guid id, UpdateProveedorRequest req, IMediator m, CancellationToken ct) =>
        {
            var result = await m.Send(new UpdateProveedorCommand(
                id, req.RazonSocial, req.Email,
                req.DiasVencimiento, req.DescuentoProntoPago, req.MetodoPago,
                req.Direccion, req.Ciudad, req.CodigoPostal, req.Pais,
                req.Telefono, req.Web), ct);
            return result.ToHttpResult();
        })
        .WithName("UpdateProveedor")
        .WithTags("Proveedores");

        group.MapDelete("/{id:guid}", async (Guid id, IMediator m, CancellationToken ct) =>
        {
            var result = await m.Send(new DeleteProveedorCommand(id), ct);
            return result.ToHttpResult();
        })
        .WithName("DeleteProveedor")
        .WithTags("Proveedores");
    }
}

public record UpdateProveedorRequest(
    string RazonSocial,
    string Email,
    int DiasVencimiento,
    decimal DescuentoProntoPago,
    string MetodoPago,
    string? Direccion,
    string? Ciudad,
    string? CodigoPostal,
    string? Pais,
    string? Telefono,
    string? Web);
