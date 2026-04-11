using MediatR;
using Microsoft.EntityFrameworkCore;
using PlanTA.Inventario.Application.Interfaces;
using PlanTA.Inventario.Domain.Entities;
using PlanTA.Inventario.Domain.Errors;
using PlanTA.SharedKernel;

namespace PlanTA.Inventario.Application.Features.Lotes.CreateLote;

public sealed class CreateLoteCommandHandler(
    IInventarioDbContext db,
    ICurrentTenant tenant)
    : IRequestHandler<CreateLoteCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateLoteCommand request, CancellationToken cancellationToken)
    {
        var productoExists = await db.Productos
            .AnyAsync(p => p.Id == new ProductoId(request.ProductoId) && p.EmpresaId == tenant.EmpresaId,
                cancellationToken);

        if (!productoExists)
            return Result<Guid>.Failure(ProductoErrors.NotFound(request.ProductoId));

        var lote = Lote.Crear(
            new ProductoId(request.ProductoId),
            request.Cantidad,
            tenant.EmpresaId,
            request.CodigoLote,
            request.FechaCaducidad,
            request.Origen,
            request.Notas);

        db.Lotes.Add(lote);
        await db.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(lote.Id.Value);
    }
}
