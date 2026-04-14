using MediatR;
using PlanTA.Inventario.Application.Features.Almacenes.CreateAlmacen;
using PlanTA.Inventario.Application.Features.Almacenes.DeleteAlmacen;
using PlanTA.Inventario.Application.Features.Almacenes.GetAlmacen;
using PlanTA.Inventario.Application.Features.Almacenes.ListAlmacenes;
using PlanTA.SharedKernel.Extensions;

namespace PlanTA.API.Endpoints.Inventario;

public sealed class AlmacenesEndpoints : IEndpointGroup
{
    public string? RoutePrefix => "/api/v1/inventario/almacenes";

    public void MapEndpoints(RouteGroupBuilder group)
    {
        group.MapGet("/", async (IMediator m, CancellationToken ct) =>
        {
            var result = await m.Send(new ListAlmacenesQuery(), ct);
            return result.ToHttpResult();
        })
        .WithName("ListAlmacenes")
        .WithTags("Almacenes");

        group.MapGet("/{id:guid}", async (Guid id, IMediator m, CancellationToken ct) =>
        {
            var result = await m.Send(new GetAlmacenQuery(id), ct);
            return result.ToHttpResult();
        })
        .WithName("GetAlmacen")
        .WithTags("Almacenes");

        group.MapPost("/", async (CreateAlmacenCommand command, IMediator m, CancellationToken ct) =>
        {
            var result = await m.Send(command, ct);
            return result.ToHttpResult(201);
        })
        .WithName("CreateAlmacen")
        .WithTags("Almacenes");

        group.MapDelete("/{id:guid}", async (Guid id, IMediator m, CancellationToken ct) =>
        {
            var result = await m.Send(new DeleteAlmacenCommand(id), ct);
            return result.ToHttpResult();
        })
        .WithName("DeleteAlmacen")
        .WithTags("Almacenes");
    }
}
