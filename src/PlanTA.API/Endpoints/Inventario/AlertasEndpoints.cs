using MediatR;
using PlanTA.Inventario.Application.Features.Alertas.CreateAlerta;
using PlanTA.Inventario.Application.Features.Alertas.ListAlertas;
using PlanTA.SharedKernel.Extensions;

namespace PlanTA.API.Endpoints.Inventario;

public sealed class AlertasEndpoints : IEndpointGroup
{
    public string? RoutePrefix => "/api/v1/inventario/alertas";

    public void MapEndpoints(RouteGroupBuilder group)
    {
        group.MapGet("/", async (Guid? productoId, IMediator m, CancellationToken ct) =>
        {
            var result = await m.Send(new ListAlertasQuery(productoId), ct);
            return result.ToHttpResult();
        })
        .WithName("ListAlertas")
        .WithTags("Alertas");

        group.MapPost("/", async (CreateAlertaCommand command, IMediator m, CancellationToken ct) =>
        {
            var result = await m.Send(command, ct);
            return result.ToHttpResult(201);
        })
        .WithName("CreateAlerta")
        .WithTags("Alertas");
    }
}
