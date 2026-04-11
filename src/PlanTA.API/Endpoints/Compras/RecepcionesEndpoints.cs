using MediatR;
using PlanTA.Compras.Application.Features.Recepciones.CambiarEstadoRecepcion;
using PlanTA.Compras.Application.Features.Recepciones.CreateRecepcion;
using PlanTA.Compras.Application.Features.Recepciones.GetRecepcion;
using PlanTA.Compras.Application.Features.Recepciones.ListRecepciones;
using PlanTA.Compras.Domain.Enums;
using PlanTA.SharedKernel.Extensions;

namespace PlanTA.API.Endpoints.Compras;

public sealed class RecepcionesEndpoints : IEndpointGroup
{
    public string? RoutePrefix => "/api/v1/compras/recepciones";

    public void MapEndpoints(RouteGroupBuilder group)
    {
        group.MapGet("/", async (Guid? ordenCompraId, EstadoRecepcion? estado, int? page, int? pageSize, IMediator m, CancellationToken ct) =>
        {
            var result = await m.Send(new ListRecepcionesQuery(ordenCompraId, estado, page ?? 1, pageSize ?? 20), ct);
            return result.ToHttpResult();
        })
        .WithName("ListRecepciones")
        .WithTags("Recepciones");

        group.MapGet("/{id:guid}", async (Guid id, IMediator m, CancellationToken ct) =>
        {
            var result = await m.Send(new GetRecepcionQuery(id), ct);
            return result.ToHttpResult();
        })
        .WithName("GetRecepcion")
        .WithTags("Recepciones");

        group.MapPost("/", async (CreateRecepcionCommand command, IMediator m, CancellationToken ct) =>
        {
            var result = await m.Send(command, ct);
            return result.ToHttpResult(201);
        })
        .WithName("CreateRecepcion")
        .WithTags("Recepciones");

        group.MapPut("/{id:guid}/estado", async (Guid id, CambiarEstadoRecepcionRequest req, IMediator m, CancellationToken ct) =>
        {
            var result = await m.Send(new CambiarEstadoRecepcionCommand(id, req.EstadoDestino, req.Motivo), ct);
            return result.ToHttpResult();
        })
        .WithName("CambiarEstadoRecepcion")
        .WithTags("Recepciones");
    }
}

public record CambiarEstadoRecepcionRequest(EstadoRecepcion EstadoDestino, string? Motivo = null);
