using MediatR;
using PlanTA.Mantenimiento.Application.Features.OrdenesTrabajo.CompletarOT;
using PlanTA.Mantenimiento.Application.Features.OrdenesTrabajo.CreateOrdenTrabajo;
using PlanTA.Mantenimiento.Application.Features.OrdenesTrabajo.DeleteOT;
using PlanTA.Mantenimiento.Application.Features.OrdenesTrabajo.GetOrdenTrabajo;
using PlanTA.Mantenimiento.Application.Features.OrdenesTrabajo.ListOrdenesTrabajo;
using PlanTA.Mantenimiento.Application.Features.Planes.CreatePlan;
using PlanTA.Mantenimiento.Domain.Enums;
using PlanTA.SharedKernel.Extensions;

namespace PlanTA.API.Endpoints.Mantenimiento;

public sealed class OrdenesTrabajoEndpoints : IEndpointGroup
{
    public string? RoutePrefix => "/api/v1/mantenimiento/ordenes-trabajo";

    public void MapEndpoints(RouteGroupBuilder group)
    {
        group.MapGet("/", async (EstadoOT? estado, TipoMantenimiento? tipo, Guid? activoId,
            int? page, int? pageSize, IMediator m, CancellationToken ct) =>
        {
            var result = await m.Send(new ListOrdenesTrabajoQuery(estado, tipo, activoId, page ?? 1, pageSize ?? 20), ct);
            return result.ToHttpResult();
        }).WithName("ListOrdenesTrabajo").WithTags("Mantenimiento");

        group.MapGet("/{id:guid}", async (Guid id, IMediator m, CancellationToken ct) =>
        {
            var result = await m.Send(new GetOrdenTrabajoQuery(id), ct);
            return result.ToHttpResult();
        }).WithName("GetOrdenTrabajo").WithTags("Mantenimiento");

        group.MapPost("/", async (CreateOrdenTrabajoCommand cmd, IMediator m, CancellationToken ct) =>
        {
            var result = await m.Send(cmd, ct);
            return result.ToHttpResult(201);
        }).WithName("CreateOrdenTrabajo").WithTags("Mantenimiento");

        group.MapPost("/{id:guid}/completar", async (Guid id, CompletarOTRequest req, IMediator m, CancellationToken ct) =>
        {
            var result = await m.Send(new CompletarOTCommand(id, req.HorasReales, req.CosteManoObra, req.NotasCierre), ct);
            return result.ToHttpResult();
        }).WithName("CompletarOrdenTrabajo").WithTags("Mantenimiento");

        group.MapDelete("/{id:guid}", async (Guid id, IMediator m, CancellationToken ct) =>
        {
            var result = await m.Send(new DeleteOTCommand(id), ct);
            return result.ToHttpResult();
        }).WithName("DeleteOrdenTrabajo").WithTags("Mantenimiento");
    }
}

public record CompletarOTRequest(decimal HorasReales, decimal CosteManoObra, string? NotasCierre);

public sealed class PlanesMantenimientoEndpoints : IEndpointGroup
{
    public string? RoutePrefix => "/api/v1/mantenimiento/planes";

    public void MapEndpoints(RouteGroupBuilder group)
    {
        group.MapPost("/", async (CreatePlanCommand cmd, IMediator m, CancellationToken ct) =>
        {
            var result = await m.Send(cmd, ct);
            return result.ToHttpResult(201);
        }).WithName("CreatePlanMantenimiento").WithTags("Mantenimiento");
    }
}
