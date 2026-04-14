using MediatR;
using Microsoft.EntityFrameworkCore;
using PlanTA.Ventas.Application.Interfaces;
using PlanTA.Ventas.Domain.Entities;
using PlanTA.Ventas.Domain.Errors;
using PlanTA.SharedKernel;

namespace PlanTA.Ventas.Application.Features.Clientes.DeleteCliente;

public record DeleteClienteCommand(Guid ClienteId) : IRequest<Result<Guid>>;

public sealed class DeleteClienteCommandHandler(
    IVentasDbContext db,
    ICurrentTenant tenant)
    : IRequestHandler<DeleteClienteCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(DeleteClienteCommand request, CancellationToken cancellationToken)
    {
        var entity = await db.Clientes
            .FirstOrDefaultAsync(c => c.Id == new ClienteId(request.ClienteId) && c.EmpresaId == tenant.EmpresaId,
                cancellationToken);

        if (entity is null)
            return Result<Guid>.Failure(ClienteErrors.NotFound(request.ClienteId));

        entity.SoftDelete();
        await db.SaveChangesAsync(cancellationToken);
        return Result<Guid>.Success(entity.Id.Value);
    }
}
