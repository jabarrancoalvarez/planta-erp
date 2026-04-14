using MediatR;
using Microsoft.EntityFrameworkCore;
using PlanTA.Costes.Application.Interfaces;
using PlanTA.Costes.Domain.Entities;
using PlanTA.Costes.Domain.Errors;
using PlanTA.SharedKernel;

namespace PlanTA.Costes.Application.Features.Imputaciones.UpdateImputacion;

public sealed class UpdateImputacionCommandHandler(
    ICostesDbContext db,
    ICurrentTenant tenant)
    : IRequestHandler<UpdateImputacionCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(UpdateImputacionCommand request, CancellationToken ct)
    {
        var imp = await db.Imputaciones
            .FirstOrDefaultAsync(i => i.Id == new ImputacionCosteId(request.ImputacionId) && i.EmpresaId == tenant.EmpresaId, ct);
        if (imp is null)
            return Result<bool>.Failure(ImputacionCosteErrors.NotFound(request.ImputacionId));
        var result = imp.Editar(request.Cantidad, request.PrecioUnitario, request.Concepto, request.Fecha);
        if (result.IsFailure) return result;
        await db.SaveChangesAsync(ct);
        return Result<bool>.Success(true);
    }
}
