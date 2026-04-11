using MediatR;
using Microsoft.EntityFrameworkCore;
using PlanTA.Inventario.Application.Interfaces;
using PlanTA.Inventario.Domain.Entities;
using PlanTA.Inventario.Domain.Errors;
using PlanTA.SharedKernel;

namespace PlanTA.Inventario.Application.Features.Alertas.CreateAlerta;

public sealed class CreateAlertaCommandHandler(
    IInventarioDbContext db,
    ICurrentTenant tenant)
    : IRequestHandler<CreateAlertaCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateAlertaCommand request, CancellationToken cancellationToken)
    {
        var productoExists = await db.Productos
            .AnyAsync(p => p.Id == new ProductoId(request.ProductoId) && p.EmpresaId == tenant.EmpresaId,
                cancellationToken);

        if (!productoExists)
            return Result<Guid>.Failure(ProductoErrors.NotFound(request.ProductoId));

        var alerta = AlertaStock.Crear(
            new ProductoId(request.ProductoId),
            request.StockMinimo,
            request.StockMaximo,
            tenant.EmpresaId,
            request.AlmacenId.HasValue ? new AlmacenId(request.AlmacenId.Value) : null,
            request.PuntoReorden,
            request.CantidadReorden,
            request.AutoReorden);

        db.Alertas.Add(alerta);
        await db.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(alerta.Id.Value);
    }
}
