using MediatR;
using Microsoft.EntityFrameworkCore;
using PlanTA.Calidad.Application.Interfaces;
using PlanTA.Calidad.Domain.Entities;
using PlanTA.Calidad.Domain.Errors;
using PlanTA.SharedKernel;
using PlanTA.SharedKernel.CQRS;

namespace PlanTA.Calidad.Application.Features.Inspecciones.DeleteInspeccion;

public record DeleteInspeccionCommand(Guid InspeccionId) : ICommand<Guid>;

public sealed class DeleteInspeccionCommandHandler(
    ICalidadDbContext db,
    ICurrentTenant tenant)
    : IRequestHandler<DeleteInspeccionCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(DeleteInspeccionCommand request, CancellationToken cancellationToken)
    {
        var insp = await db.Inspecciones
            .FirstOrDefaultAsync(
                i => i.Id == new InspeccionId(request.InspeccionId) && i.EmpresaId == tenant.EmpresaId,
                cancellationToken);

        if (insp is null)
            return Result<Guid>.Failure(InspeccionErrors.NotFound(request.InspeccionId));

        insp.SoftDelete();
        await db.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(insp.Id.Value);
    }
}
