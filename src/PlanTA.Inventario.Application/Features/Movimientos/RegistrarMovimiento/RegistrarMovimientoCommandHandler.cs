using MediatR;
using Microsoft.EntityFrameworkCore;
using PlanTA.Inventario.Application.Interfaces;
using PlanTA.Inventario.Domain.Entities;
using PlanTA.Inventario.Domain.Errors;
using PlanTA.SharedKernel;

namespace PlanTA.Inventario.Application.Features.Movimientos.RegistrarMovimiento;

public sealed class RegistrarMovimientoCommandHandler(
    IInventarioDbContext db,
    ICurrentTenant tenant)
    : IRequestHandler<RegistrarMovimientoCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(RegistrarMovimientoCommand request, CancellationToken cancellationToken)
    {
        if (request.Cantidad <= 0)
            return Result<Guid>.Failure(MovimientoErrors.CantidadInvalida);

        var producto = await db.Productos
            .FirstOrDefaultAsync(p => p.Id == new ProductoId(request.ProductoId) && p.EmpresaId == tenant.EmpresaId,
                cancellationToken);

        if (producto is null)
            return Result<Guid>.Failure(ProductoErrors.NotFound(request.ProductoId));

        // Get current stock from last movement for this product-almacen
        var cantidadAnterior = await db.Movimientos
            .Where(m => m.ProductoId == new ProductoId(request.ProductoId)
                && m.AlmacenId == new AlmacenId(request.AlmacenId)
                && m.EmpresaId == tenant.EmpresaId)
            .OrderByDescending(m => m.CreatedAt)
            .Select(m => m.CantidadPosterior)
            .FirstOrDefaultAsync(cancellationToken);

        var movimiento = MovimientoStock.Registrar(
            new ProductoId(request.ProductoId),
            new AlmacenId(request.AlmacenId),
            request.Tipo,
            request.Cantidad,
            cantidadAnterior,
            tenant.EmpresaId,
            request.UbicacionId.HasValue ? new UbicacionId(request.UbicacionId.Value) : null,
            request.LoteId.HasValue ? new LoteId(request.LoteId.Value) : null,
            request.Referencia,
            request.Notas);

        db.Movimientos.Add(movimiento);
        await db.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(movimiento.Id.Value);
    }
}
