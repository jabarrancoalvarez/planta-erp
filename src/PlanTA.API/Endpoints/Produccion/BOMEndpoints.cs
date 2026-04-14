using MediatR;
using PlanTA.Produccion.Application.Features.BOM.AddLineaBOM;
using PlanTA.Produccion.Application.Features.BOM.CreateBOM;
using PlanTA.Produccion.Application.Features.BOM.DeleteBOM;
using PlanTA.Produccion.Application.Features.BOM.GetBOM;
using PlanTA.Produccion.Application.Features.BOM.ListBOMs;
using PlanTA.SharedKernel.Extensions;

namespace PlanTA.API.Endpoints.Produccion;

public sealed class BOMEndpoints : IEndpointGroup
{
    public string? RoutePrefix => "/api/v1/produccion/bom";

    public void MapEndpoints(RouteGroupBuilder group)
    {
        group.MapGet("/", async (string? search, int? page, int? pageSize, IMediator m, CancellationToken ct) =>
        {
            var result = await m.Send(new ListBOMsQuery(search, page ?? 1, pageSize ?? 20), ct);
            return result.ToHttpResult();
        })
        .WithName("ListBOMs")
        .WithTags("BOM");

        group.MapGet("/{id:guid}", async (Guid id, IMediator m, CancellationToken ct) =>
        {
            var result = await m.Send(new GetBOMQuery(id), ct);
            return result.ToHttpResult();
        })
        .WithName("GetBOM")
        .WithTags("BOM");

        group.MapPost("/", async (CreateBOMCommand command, IMediator m, CancellationToken ct) =>
        {
            var result = await m.Send(command, ct);
            return result.ToHttpResult(201);
        })
        .WithName("CreateBOM")
        .WithTags("BOM");

        group.MapPost("/{id:guid}/lineas", async (Guid id, AddLineaBOMRequest req, IMediator m, CancellationToken ct) =>
        {
            var result = await m.Send(new AddLineaBOMCommand(
                id, req.ComponenteProductoId, req.Cantidad, req.UnidadMedida, req.Merma, req.Orden), ct);
            return result.ToHttpResult(201);
        })
        .WithName("AddLineaBOM")
        .WithTags("BOM");

        group.MapDelete("/{id:guid}", async (Guid id, IMediator m, CancellationToken ct) =>
        {
            var result = await m.Send(new DeleteBOMCommand(id), ct);
            return result.ToHttpResult();
        })
        .WithName("DeleteBOM")
        .WithTags("BOM");
    }
}

public record AddLineaBOMRequest(
    Guid ComponenteProductoId,
    decimal Cantidad,
    string UnidadMedida,
    decimal Merma = 0,
    int? Orden = null);
