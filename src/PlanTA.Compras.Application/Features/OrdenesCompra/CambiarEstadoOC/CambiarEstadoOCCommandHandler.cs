using MediatR;
using Microsoft.EntityFrameworkCore;
using PlanTA.Compras.Application.Interfaces;
using PlanTA.Compras.Domain.Entities;
using PlanTA.Compras.Domain.Enums;
using PlanTA.Compras.Domain.Errors;
using PlanTA.SharedKernel;

namespace PlanTA.Compras.Application.Features.OrdenesCompra.CambiarEstadoOC;

public sealed class CambiarEstadoOCCommandHandler(
    IComprasDbContext db,
    ICurrentTenant tenant)
    : IRequestHandler<CambiarEstadoOCCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(CambiarEstadoOCCommand request, CancellationToken cancellationToken)
    {
        var oc = await db.OrdenesCompra
            .Include(o => o.Lineas)
            .Where(o => o.Id == new OrdenCompraId(request.OrdenCompraId) && o.EmpresaId == tenant.EmpresaId)
            .FirstOrDefaultAsync(cancellationToken);

        if (oc is null)
            return Result<bool>.Failure(OrdenCompraErrors.NotFound(request.OrdenCompraId));

        var result = request.EstadoDestino switch
        {
            EstadoOC.Enviada => oc.Enviar(),
            EstadoOC.ParcialmenteRecibida => oc.RecibirParcialmente(),
            EstadoOC.Recibida => oc.RecibirCompleta(),
            EstadoOC.Cancelada => oc.Cancelar(request.Motivo ?? "Sin motivo especificado"),
            _ => Result<bool>.Failure(
                OrdenCompraErrors.TransicionInvalida(oc.EstadoOC.ToString(), request.EstadoDestino.ToString()))
        };

        if (result.IsFailure)
            return result;

        await db.SaveChangesAsync(cancellationToken);
        return Result<bool>.Success(true);
    }
}
