using MediatR;
using Microsoft.EntityFrameworkCore;
using PlanTA.CRM.Application.Interfaces;
using PlanTA.CRM.Domain.Entities;
using PlanTA.CRM.Domain.Errors;
using PlanTA.SharedKernel;

namespace PlanTA.CRM.Application.Features.Oportunidades.UpdateOportunidad;

public sealed class UpdateOportunidadCommandHandler(
    ICrmDbContext db,
    ICurrentTenant tenant)
    : IRequestHandler<UpdateOportunidadCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(UpdateOportunidadCommand request, CancellationToken ct)
    {
        var op = await db.Oportunidades
            .Where(o => o.Id == new OportunidadId(request.OportunidadId) && o.EmpresaId == tenant.EmpresaId)
            .FirstOrDefaultAsync(ct);
        if (op is null)
            return Result<bool>.Failure(OportunidadErrors.NotFound(request.OportunidadId));
        var result = op.Editar(request.Titulo, request.ImporteEstimado, request.FechaCierreEstimada, request.Descripcion);
        if (result.IsFailure) return result;
        await db.SaveChangesAsync(ct);
        return Result<bool>.Success(true);
    }
}
