using MediatR;
using PlanTA.Inventario.Application.Features.Lotes.CreateLote;
using PlanTA.Inventario.Application.Features.Lotes.ListLotes;
using PlanTA.SharedKernel.Extensions;

namespace PlanTA.API.Endpoints.Inventario;

public sealed class LotesEndpoints : IEndpointGroup
{
    public string? RoutePrefix => "/api/v1/inventario/lotes";

    public void MapEndpoints(RouteGroupBuilder group)
    {
        group.MapGet("/", async (Guid? productoId, int? page, int? pageSize, IMediator m, CancellationToken ct) =>
        {
            var result = await m.Send(new ListLotesQuery(productoId, page ?? 1, pageSize ?? 20), ct);
            return result.ToHttpResult();
        })
        .WithName("ListLotes")
        .WithTags("Lotes");

        group.MapPost("/", async (CreateLoteCommand command, IMediator m, CancellationToken ct) =>
        {
            var result = await m.Send(command, ct);
            return result.ToHttpResult(201);
        })
        .WithName("CreateLote")
        .WithTags("Lotes");
    }
}
