using MediatR;
using PlanTA.CRM.Application.Interfaces;
using PlanTA.CRM.Domain.Entities;
using PlanTA.SharedKernel;

namespace PlanTA.CRM.Application.Features.Leads.CreateLead;

public sealed class CreateLeadCommandHandler(
    ICrmDbContext db,
    ICurrentTenant tenant)
    : IRequestHandler<CreateLeadCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateLeadCommand request, CancellationToken ct)
    {
        var lead = Lead.Crear(
            request.Nombre, request.Origen, tenant.EmpresaId,
            request.Empresa, request.Email, request.Telefono,
            request.Notas, request.AsignadoAUserId);

        db.Leads.Add(lead);
        await db.SaveChangesAsync(ct);
        return Result<Guid>.Success(lead.Id.Value);
    }
}
