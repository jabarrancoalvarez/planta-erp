using MediatR;
using Microsoft.EntityFrameworkCore;
using PlanTA.Activos.Application.Interfaces;
using PlanTA.Activos.Domain.Entities;
using PlanTA.Activos.Domain.Errors;
using PlanTA.SharedKernel;

namespace PlanTA.Activos.Application.Features.Activos.DeleteActivo;

public record DeleteActivoCommand(Guid ActivoId) : IRequest<Result<Guid>>;

public sealed class DeleteActivoCommandHandler(
    IActivosDbContext db,
    ICurrentTenant tenant)
    : IRequestHandler<DeleteActivoCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(DeleteActivoCommand request, CancellationToken cancellationToken)
    {
        var entity = await db.Activos
            .FirstOrDefaultAsync(a => a.Id == new ActivoId(request.ActivoId) && a.EmpresaId == tenant.EmpresaId,
                cancellationToken);

        if (entity is null)
            return Result<Guid>.Failure(ActivoErrors.NotFound(request.ActivoId));

        entity.SoftDelete();
        await db.SaveChangesAsync(cancellationToken);
        return Result<Guid>.Success(entity.Id.Value);
    }
}
