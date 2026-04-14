using MediatR;
using PlanTA.CRM.Application.Features.Actividades.CreateActividad;
using PlanTA.CRM.Application.Features.Leads.CreateLead;
using PlanTA.CRM.Application.Features.Leads.ListLeads;
using PlanTA.CRM.Application.Features.Oportunidades.AvanzarFase;
using PlanTA.CRM.Application.Features.Oportunidades.CreateOportunidad;
using PlanTA.CRM.Application.Features.Oportunidades.ListOportunidades;
using PlanTA.CRM.Domain.Enums;
using PlanTA.SharedKernel.Extensions;

namespace PlanTA.API.Endpoints.CRM;

public sealed class CRMEndpoints : IEndpointGroup
{
    public string? RoutePrefix => "/api/v1/crm";

    public void MapEndpoints(RouteGroupBuilder group)
    {
        // Leads
        group.MapGet("/leads", async (string? search, EstadoLead? estado, int? page, int? pageSize,
            IMediator m, CancellationToken ct) =>
        {
            var r = await m.Send(new ListLeadsQuery(search, estado, page ?? 1, pageSize ?? 20), ct);
            return r.ToHttpResult();
        }).WithName("ListLeads").WithTags("CRM");

        group.MapPost("/leads", async (CreateLeadCommand cmd, IMediator m, CancellationToken ct) =>
            (await m.Send(cmd, ct)).ToHttpResult(201)).WithName("CreateLead").WithTags("CRM");

        // Oportunidades
        group.MapGet("/oportunidades", async (FaseOportunidad? fase, int? page, int? pageSize,
            IMediator m, CancellationToken ct) =>
        {
            var r = await m.Send(new ListOportunidadesQuery(fase, page ?? 1, pageSize ?? 20), ct);
            return r.ToHttpResult();
        }).WithName("ListOportunidades").WithTags("CRM");

        group.MapPost("/oportunidades", async (CreateOportunidadCommand cmd, IMediator m, CancellationToken ct) =>
            (await m.Send(cmd, ct)).ToHttpResult(201)).WithName("CreateOportunidad").WithTags("CRM");

        group.MapPost("/oportunidades/{id:guid}/avanzar", async (Guid id, AvanzarFaseRequest req,
            IMediator m, CancellationToken ct) =>
            (await m.Send(new AvanzarFaseCommand(id, req.NuevaFase, req.ProbabilidadPct), ct)).ToHttpResult())
            .WithName("AvanzarFaseOportunidad").WithTags("CRM");

        // Actividades
        group.MapPost("/actividades", async (CreateActividadCommand cmd, IMediator m, CancellationToken ct) =>
            (await m.Send(cmd, ct)).ToHttpResult(201)).WithName("CreateActividadCrm").WithTags("CRM");
    }
}

public record AvanzarFaseRequest(FaseOportunidad NuevaFase, int? ProbabilidadPct);
