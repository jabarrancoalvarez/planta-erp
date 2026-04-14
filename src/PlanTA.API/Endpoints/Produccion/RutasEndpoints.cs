using MediatR;
using PlanTA.Produccion.Application.Features.Rutas.CreateRuta;
using PlanTA.Produccion.Application.Features.Rutas.DeleteRuta;
using PlanTA.Produccion.Application.Features.Rutas.GetRuta;
using PlanTA.Produccion.Application.Features.Rutas.ListRutas;
using PlanTA.Produccion.Application.Features.Rutas.UpdateRuta;
using PlanTA.SharedKernel.Extensions;

namespace PlanTA.API.Endpoints.Produccion;

public sealed class RutasEndpoints : IEndpointGroup
{
    public string? RoutePrefix => "/api/v1/produccion/rutas";

    public void MapEndpoints(RouteGroupBuilder group)
    {
        group.MapGet("/", async (string? search, int? page, int? pageSize, IMediator m, CancellationToken ct) =>
        {
            var result = await m.Send(new ListRutasQuery(search, page ?? 1, pageSize ?? 20), ct);
            return result.ToHttpResult();
        })
        .WithName("ListRutas")
        .WithTags("Rutas");

        group.MapGet("/{id:guid}", async (Guid id, IMediator m, CancellationToken ct) =>
        {
            var result = await m.Send(new GetRutaQuery(id), ct);
            return result.ToHttpResult();
        })
        .WithName("GetRuta")
        .WithTags("Rutas");

        group.MapPost("/", async (CreateRutaCommand command, IMediator m, CancellationToken ct) =>
        {
            var result = await m.Send(command, ct);
            return result.ToHttpResult(201);
        })
        .WithName("CreateRuta")
        .WithTags("Rutas");

        group.MapPut("/{id:guid}", async (Guid id, UpdateRutaRequest req, IMediator m, CancellationToken ct) =>
        {
            var result = await m.Send(new UpdateRutaCommand(id, req.Nombre, req.Descripcion), ct);
            return result.ToHttpResult();
        })
        .WithName("UpdateRuta")
        .WithTags("Rutas");

        group.MapDelete("/{id:guid}", async (Guid id, IMediator m, CancellationToken ct) =>
        {
            var result = await m.Send(new DeleteRutaCommand(id), ct);
            return result.ToHttpResult();
        })
        .WithName("DeleteRuta")
        .WithTags("Rutas");
    }
}

public record UpdateRutaRequest(string Nombre, string? Descripcion);
