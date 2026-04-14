using MediatR;
using Microsoft.EntityFrameworkCore;
using PlanTA.Compras.Application.Interfaces;
using PlanTA.Compras.Domain.Entities;
using PlanTA.Compras.Domain.Errors;
using PlanTA.SharedKernel;

namespace PlanTA.Compras.Application.Features.Proveedores.DeleteProveedor;

public record DeleteProveedorCommand(Guid ProveedorId) : IRequest<Result<Guid>>;

public sealed class DeleteProveedorCommandHandler(
    IComprasDbContext db,
    ICurrentTenant tenant)
    : IRequestHandler<DeleteProveedorCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(DeleteProveedorCommand request, CancellationToken cancellationToken)
    {
        var entity = await db.Proveedores
            .FirstOrDefaultAsync(p => p.Id == new ProveedorId(request.ProveedorId) && p.EmpresaId == tenant.EmpresaId,
                cancellationToken);

        if (entity is null)
            return Result<Guid>.Failure(ProveedorErrors.NotFound(request.ProveedorId));

        entity.SoftDelete();
        await db.SaveChangesAsync(cancellationToken);
        return Result<Guid>.Success(entity.Id.Value);
    }
}
