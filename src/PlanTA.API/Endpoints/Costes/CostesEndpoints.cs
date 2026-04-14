using MediatR;
using PlanTA.Costes.Application.Features.Imputaciones.CreateImputacion;
using PlanTA.Costes.Application.Features.Imputaciones.DeleteImputacion;
using PlanTA.Costes.Application.Features.Imputaciones.ListImputaciones;
using PlanTA.Costes.Application.Features.Imputaciones.ResumenOF;
using PlanTA.Costes.Application.Features.Imputaciones.UpdateImputacion;
using PlanTA.SharedKernel.Extensions;

namespace PlanTA.API.Endpoints.Costes;

public sealed class CostesEndpoints : IEndpointGroup
{
    public string? RoutePrefix => "/api/v1/costes";

    public void MapEndpoints(RouteGroupBuilder group)
    {
        group.MapGet("/imputaciones", async (Guid? ordenFabricacionId, Guid? ordenTrabajoId,
            int? page, int? pageSize, IMediator m, CancellationToken ct) =>
        {
            var result = await m.Send(new ListImputacionesQuery(
                ordenFabricacionId, ordenTrabajoId, page ?? 1, pageSize ?? 50), ct);
            return result.ToHttpResult();
        }).WithName("ListImputaciones").WithTags("Costes");

        group.MapPost("/imputaciones", async (CreateImputacionCommand cmd, IMediator m, CancellationToken ct) =>
        {
            var result = await m.Send(cmd, ct);
            return result.ToHttpResult(201);
        }).WithName("CreateImputacion").WithTags("Costes");

        group.MapPut("/imputaciones/{id:guid}", async (Guid id, UpdateImputacionRequest req, IMediator m, CancellationToken ct) =>
            (await m.Send(new UpdateImputacionCommand(id, req.Cantidad, req.PrecioUnitario, req.Concepto, req.Fecha), ct)).ToHttpResult())
            .WithName("UpdateImputacion").WithTags("Costes");

        group.MapDelete("/imputaciones/{id:guid}", async (Guid id, IMediator m, CancellationToken ct) =>
            (await m.Send(new DeleteImputacionCommand(id), ct)).ToHttpResult())
            .WithName("DeleteImputacion").WithTags("Costes");

        group.MapGet("/ordenes-fabricacion/{id:guid}/resumen", async (Guid id, IMediator m, CancellationToken ct) =>
        {
            var result = await m.Send(new ResumenOFQuery(id), ct);
            return result.ToHttpResult();
        }).WithName("ResumenOF").WithTags("Costes");
    }
}

public record UpdateImputacionRequest(decimal Cantidad, decimal PrecioUnitario, string? Concepto, DateTimeOffset Fecha);
