using MediatR;
using Microsoft.EntityFrameworkCore;
using PlanTA.CRM.Application.Interfaces;
using PlanTA.CRM.Domain.Entities;
using PlanTA.CRM.Domain.Errors;
using PlanTA.SharedKernel;

namespace PlanTA.CRM.Application.Features.Oportunidades.AvanzarFase;

public sealed class AvanzarFaseCommandHandler(
    ICrmDbContext db,
    ICurrentTenant tenant)
    : IRequestHandler<AvanzarFaseCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(AvanzarFaseCommand request, CancellationToken ct)
    {
        var id = new OportunidadId(request.Id);
        var op = await db.Oportunidades
            .FirstOrDefaultAsync(o => o.Id == id && o.EmpresaId == tenant.EmpresaId, ct);

        if (op is null) return Result<bool>.Failure(OportunidadErrors.NotFound(request.Id));

        op.AvanzarFase(request.NuevaFase, request.ProbabilidadPct);
        await db.SaveChangesAsync(ct);
        return Result<bool>.Success(true);
    }
}
