using MediatR;
using Microsoft.EntityFrameworkCore;
using PlanTA.CRM.Application.Interfaces;
using PlanTA.CRM.Domain.Entities;
using PlanTA.CRM.Domain.Errors;
using PlanTA.SharedKernel;

namespace PlanTA.CRM.Application.Features.Leads.UpdateLead;

public sealed class UpdateLeadCommandHandler(
    ICrmDbContext db,
    ICurrentTenant tenant)
    : IRequestHandler<UpdateLeadCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(UpdateLeadCommand request, CancellationToken ct)
    {
        var lead = await db.Leads
            .Where(l => l.Id == new LeadId(request.LeadId) && l.EmpresaId == tenant.EmpresaId)
            .FirstOrDefaultAsync(ct);
        if (lead is null)
            return Result<bool>.Failure(LeadErrors.NotFound(request.LeadId));
        lead.Editar(request.Nombre, request.Empresa, request.Email, request.Telefono, request.Notas);
        await db.SaveChangesAsync(ct);
        return Result<bool>.Success(true);
    }
}
