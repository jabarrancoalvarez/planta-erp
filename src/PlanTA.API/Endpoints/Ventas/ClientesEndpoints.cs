using MediatR;
using PlanTA.Ventas.Application.Features.Clientes.CreateCliente;
using PlanTA.Ventas.Application.Features.Clientes.DeleteCliente;
using PlanTA.Ventas.Application.Features.Clientes.GetCliente;
using PlanTA.Ventas.Application.Features.Clientes.ListClientes;
using PlanTA.Ventas.Application.Features.Clientes.UpdateCliente;
using PlanTA.SharedKernel.Extensions;

namespace PlanTA.API.Endpoints.Ventas;

public sealed class ClientesEndpoints : IEndpointGroup
{
    public string? RoutePrefix => "/api/v1/ventas/clientes";

    public void MapEndpoints(RouteGroupBuilder group)
    {
        group.MapGet("/", async (string? search, int? page, int? pageSize, IMediator m, CancellationToken ct) =>
        {
            var result = await m.Send(new ListClientesQuery(search, page ?? 1, pageSize ?? 20), ct);
            return result.ToHttpResult();
        })
        .WithName("ListClientes")
        .WithTags("Clientes");

        group.MapGet("/{id:guid}", async (Guid id, IMediator m, CancellationToken ct) =>
        {
            var result = await m.Send(new GetClienteQuery(id), ct);
            return result.ToHttpResult();
        })
        .WithName("GetCliente")
        .WithTags("Clientes");

        group.MapPost("/", async (CreateClienteCommand command, IMediator m, CancellationToken ct) =>
        {
            var result = await m.Send(command, ct);
            return result.ToHttpResult(201);
        })
        .WithName("CreateCliente")
        .WithTags("Clientes");

        group.MapPut("/{id:guid}", async (Guid id, UpdateClienteRequest req, IMediator m, CancellationToken ct) =>
        {
            var result = await m.Send(new UpdateClienteCommand(
                id, req.RazonSocial, req.Email,
                req.DireccionEnvio, req.DireccionFacturacion,
                req.Ciudad, req.CodigoPostal, req.Pais,
                req.Telefono, req.Web), ct);
            return result.ToHttpResult();
        })
        .WithName("UpdateCliente")
        .WithTags("Clientes");

        group.MapDelete("/{id:guid}", async (Guid id, IMediator m, CancellationToken ct) =>
        {
            var result = await m.Send(new DeleteClienteCommand(id), ct);
            return result.ToHttpResult();
        })
        .WithName("DeleteCliente")
        .WithTags("Clientes");
    }
}

public record UpdateClienteRequest(
    string RazonSocial,
    string Email,
    string? DireccionEnvio,
    string? DireccionFacturacion,
    string? Ciudad,
    string? CodigoPostal,
    string? Pais,
    string? Telefono,
    string? Web);
