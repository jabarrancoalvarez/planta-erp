using MediatR;
using Microsoft.EntityFrameworkCore;
using PlanTA.Inventario.Application.Interfaces;
using PlanTA.Inventario.Domain.Entities;
using PlanTA.Inventario.Domain.Errors;
using PlanTA.SharedKernel;

namespace PlanTA.Inventario.Application.Features.Almacenes.DeleteAlmacen;

public record DeleteAlmacenCommand(Guid AlmacenId) : IRequest<Result<Guid>>;

public sealed class DeleteAlmacenCommandHandler(
    IInventarioDbContext db,
    ICurrentTenant tenant)
    : IRequestHandler<DeleteAlmacenCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(DeleteAlmacenCommand request, CancellationToken cancellationToken)
    {
        var entity = await db.Almacenes
            .FirstOrDefaultAsync(a => a.Id == new AlmacenId(request.AlmacenId) && a.EmpresaId == tenant.EmpresaId,
                cancellationToken);

        if (entity is null)
            return Result<Guid>.Failure(AlmacenErrors.NotFound(request.AlmacenId));

        entity.SoftDelete();
        await db.SaveChangesAsync(cancellationToken);
        return Result<Guid>.Success(entity.Id.Value);
    }
}
