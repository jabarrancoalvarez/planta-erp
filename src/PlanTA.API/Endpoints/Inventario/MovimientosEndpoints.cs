using MediatR;
using PlanTA.Inventario.Application.Features.Movimientos.ListMovimientos;
using PlanTA.Inventario.Application.Features.Movimientos.RegistrarMovimiento;
using PlanTA.SharedKernel.Extensions;

namespace PlanTA.API.Endpoints.Inventario;

public sealed class MovimientosEndpoints : IEndpointGroup
{
    public string? RoutePrefix => "/api/v1/inventario/movimientos";

    public void MapEndpoints(RouteGroupBuilder group)
    {
        group.MapGet("/", async (Guid? productoId, Guid? almacenId, int? page, int? pageSize, IMediator m, CancellationToken ct) =>
        {
            var result = await m.Send(new ListMovimientosQuery(productoId, almacenId, page ?? 1, pageSize ?? 20), ct);
            return result.ToHttpResult();
        })
        .WithName("ListMovimientos")
        .WithTags("Movimientos");

        group.MapPost("/", async (RegistrarMovimientoCommand command, IMediator m, CancellationToken ct) =>
        {
            var result = await m.Send(command, ct);
            return result.ToHttpResult(201);
        })
        .WithName("RegistrarMovimiento")
        .WithTags("Movimientos");
    }
}
