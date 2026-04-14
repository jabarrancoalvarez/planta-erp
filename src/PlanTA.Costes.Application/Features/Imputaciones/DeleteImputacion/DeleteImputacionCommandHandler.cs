using MediatR;
using Microsoft.EntityFrameworkCore;
using PlanTA.Costes.Application.Interfaces;
using PlanTA.Costes.Domain.Entities;
using PlanTA.Costes.Domain.Errors;
using PlanTA.SharedKernel;

namespace PlanTA.Costes.Application.Features.Imputaciones.DeleteImputacion;

public sealed class DeleteImputacionCommandHandler(
    ICostesDbContext db,
    ICurrentTenant tenant)
    : IRequestHandler<DeleteImputacionCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(DeleteImputacionCommand request, CancellationToken ct)
    {
        var imp = await db.Imputaciones
            .FirstOrDefaultAsync(i => i.Id == new ImputacionCosteId(request.ImputacionId) && i.EmpresaId == tenant.EmpresaId, ct);
        if (imp is null)
            return Result<bool>.Failure(ImputacionCosteErrors.NotFound(request.ImputacionId));
        db.Imputaciones.Remove(imp);
        await db.SaveChangesAsync(ct);
        return Result<bool>.Success(true);
    }
}
