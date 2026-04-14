using MediatR;
using Microsoft.EntityFrameworkCore;
using PlanTA.CRM.Application.Interfaces;
using PlanTA.CRM.Domain.Entities;
using PlanTA.CRM.Domain.Errors;
using PlanTA.SharedKernel;

namespace PlanTA.CRM.Application.Features.Leads.DeleteLead;

public sealed class DeleteLeadCommandHandler(
    ICrmDbContext db,
    ICurrentTenant tenant)
    : IRequestHandler<DeleteLeadCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(DeleteLeadCommand request, CancellationToken ct)
    {
        var lead = await db.Leads
            .FirstOrDefaultAsync(l => l.Id == new LeadId(request.LeadId) && l.EmpresaId == tenant.EmpresaId, ct);
        if (lead is null)
            return Result<bool>.Failure(LeadErrors.NotFound(request.LeadId));
        lead.SoftDelete();
        await db.SaveChangesAsync(ct);
        return Result<bool>.Success(true);
    }
}
