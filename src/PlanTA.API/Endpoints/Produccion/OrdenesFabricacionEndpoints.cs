using MediatR;
using PlanTA.Produccion.Application.Features.OrdenesFabricacion.CambiarEstadoOF;
using PlanTA.Produccion.Application.Features.OrdenesFabricacion.CreateOF;
using PlanTA.Produccion.Application.Features.OrdenesFabricacion.GetOF;
using PlanTA.Produccion.Application.Features.OrdenesFabricacion.ListOFs;
using PlanTA.Produccion.Application.Features.OrdenesFabricacion.RegistrarProduccion;
using PlanTA.Produccion.Domain.Enums;
using PlanTA.SharedKernel.Extensions;

namespace PlanTA.API.Endpoints.Produccion;

public sealed class OrdenesFabricacionEndpoints : IEndpointGroup
{
    public string? RoutePrefix => "/api/v1/produccion/ordenes";

    public void MapEndpoints(RouteGroupBuilder group)
    {
        group.MapGet("/", async (string? search, EstadoOF? estado, int? page, int? pageSize, IMediator m, CancellationToken ct) =>
        {
            var result = await m.Send(new ListOFsQuery(search, estado, page ?? 1, pageSize ?? 20), ct);
            return result.ToHttpResult();
        })
        .WithName("ListOrdenesFabricacion")
        .WithTags("OrdenesFabricacion");

        group.MapGet("/{id:guid}", async (Guid id, IMediator m, CancellationToken ct) =>
        {
            var result = await m.Send(new GetOFQuery(id), ct);
            return result.ToHttpResult();
        })
        .WithName("GetOrdenFabricacion")
        .WithTags("OrdenesFabricacion");

        group.MapPost("/", async (CreateOFCommand command, IMediator m, CancellationToken ct) =>
        {
            var result = await m.Send(command, ct);
            return result.ToHttpResult(201);
        })
        .WithName("CreateOrdenFabricacion")
        .WithTags("OrdenesFabricacion");

        group.MapPut("/{id:guid}/estado", async (Guid id, CambiarEstadoRequest req, IMediator m, CancellationToken ct) =>
        {
            var result = await m.Send(new CambiarEstadoOFCommand(id, req.EstadoDestino, req.Motivo), ct);
            return result.ToHttpResult();
        })
        .WithName("CambiarEstadoOF")
        .WithTags("OrdenesFabricacion");

        group.MapPost("/{id:guid}/produccion", async (Guid id, RegistrarProduccionRequest req, IMediator m, CancellationToken ct) =>
        {
            var result = await m.Send(new RegistrarProduccionCommand(
                id, req.UnidadesBuenas, req.UnidadesDefectuosas, req.Merma, req.LoteGeneradoId, req.Observaciones), ct);
            return result.ToHttpResult(201);
        })
        .WithName("RegistrarProduccion")
        .WithTags("OrdenesFabricacion");
    }
}

public record CambiarEstadoRequest(EstadoOF EstadoDestino, string? Motivo = null);

public record RegistrarProduccionRequest(
    decimal UnidadesBuenas,
    decimal UnidadesDefectuosas = 0,
    decimal Merma = 0,
    Guid? LoteGeneradoId = null,
    string? Observaciones = null);
