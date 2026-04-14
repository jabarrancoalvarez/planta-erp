using MediatR;
using Microsoft.EntityFrameworkCore;
using PlanTA.CRM.Application.Interfaces;
using PlanTA.CRM.Domain.Entities;
using PlanTA.CRM.Domain.Errors;
using PlanTA.SharedKernel;

namespace PlanTA.CRM.Application.Features.Oportunidades.DeleteOportunidad;

public sealed class DeleteOportunidadCommandHandler(
    ICrmDbContext db,
    ICurrentTenant tenant)
    : IRequestHandler<DeleteOportunidadCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(DeleteOportunidadCommand request, CancellationToken ct)
    {
        var op = await db.Oportunidades
            .FirstOrDefaultAsync(o => o.Id == new OportunidadId(request.OportunidadId) && o.EmpresaId == tenant.EmpresaId, ct);
        if (op is null)
            return Result<bool>.Failure(OportunidadErrors.NotFound(request.OportunidadId));
        op.SoftDelete();
        await db.SaveChangesAsync(ct);
        return Result<bool>.Success(true);
    }
}
