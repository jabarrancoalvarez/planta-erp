using MediatR;
using Microsoft.EntityFrameworkCore;
using PlanTA.Calidad.Application.Interfaces;
using PlanTA.Calidad.Domain.Entities;
using PlanTA.Calidad.Domain.Errors;
using PlanTA.SharedKernel;

namespace PlanTA.Calidad.Application.Features.NoConformidades.UpdateNC;

public sealed class UpdateNCCommandHandler(
    ICalidadDbContext db,
    ICurrentTenant tenant)
    : IRequestHandler<UpdateNCCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(UpdateNCCommand request, CancellationToken cancellationToken)
    {
        var nc = await db.NoConformidades
            .Where(n => n.Id == new NoConformidadId(request.NoConformidadId) && n.EmpresaId == tenant.EmpresaId)
            .FirstOrDefaultAsync(cancellationToken);

        if (nc is null)
            return Result<bool>.Failure(NoConformidadErrors.NotFound(request.NoConformidadId));

        var result = nc.Editar(request.Descripcion, request.Severidad, request.ResponsableUserId);
        if (result.IsFailure)
            return result;

        await db.SaveChangesAsync(cancellationToken);
        return Result<bool>.Success(true);
    }
}
