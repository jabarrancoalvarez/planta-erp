using MediatR;
using Microsoft.EntityFrameworkCore;
using PlanTA.Calidad.Application.Interfaces;
using PlanTA.Calidad.Domain.Entities;
using PlanTA.Calidad.Domain.Errors;
using PlanTA.SharedKernel;

namespace PlanTA.Calidad.Application.Features.NoConformidades.DeleteNC;

public sealed class DeleteNCCommandHandler(
    ICalidadDbContext db,
    ICurrentTenant tenant)
    : IRequestHandler<DeleteNCCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(DeleteNCCommand request, CancellationToken cancellationToken)
    {
        var nc = await db.NoConformidades
            .FirstOrDefaultAsync(
                n => n.Id == new NoConformidadId(request.NoConformidadId) && n.EmpresaId == tenant.EmpresaId,
                cancellationToken);

        if (nc is null)
            return Result<Guid>.Failure(NoConformidadErrors.NotFound(request.NoConformidadId));

        nc.SoftDelete();
        await db.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(nc.Id.Value);
    }
}
