using MediatR;
using PlanTA.Importador.Application.Features.Jobs.CreateJob;
using PlanTA.Importador.Application.Features.Jobs.DeleteJob;
using PlanTA.Importador.Application.Features.Jobs.ListJobs;
using PlanTA.SharedKernel.Extensions;

namespace PlanTA.API.Endpoints.Importador;

public sealed class ImportadorEndpoints : IEndpointGroup
{
    public string? RoutePrefix => "/api/v1/importador";

    public void MapEndpoints(RouteGroupBuilder group)
    {
        group.MapGet("/jobs", async (int? page, int? pageSize, IMediator m, CancellationToken ct) =>
        {
            var result = await m.Send(new ListImportJobsQuery(page ?? 1, pageSize ?? 20), ct);
            return result.ToHttpResult();
        }).WithName("ListImportJobs").WithTags("Importador");

        group.MapPost("/jobs", async (CreateImportJobCommand cmd, IMediator m, CancellationToken ct) =>
        {
            var result = await m.Send(cmd, ct);
            return result.ToHttpResult(201);
        }).WithName("CreateImportJob").WithTags("Importador");

        group.MapDelete("/jobs/{id:guid}", async (Guid id, IMediator m, CancellationToken ct) =>
            (await m.Send(new DeleteImportJobCommand(id), ct)).ToHttpResult())
            .WithName("DeleteImportJob").WithTags("Importador");
    }
}
