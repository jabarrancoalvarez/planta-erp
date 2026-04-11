using MediatR;
using Microsoft.EntityFrameworkCore;
using PlanTA.Compras.Application.Interfaces;
using PlanTA.Compras.Domain.Entities;
using PlanTA.Compras.Domain.Enums;
using PlanTA.Compras.Domain.Errors;
using PlanTA.SharedKernel;

namespace PlanTA.Compras.Application.Features.Recepciones.CambiarEstadoRecepcion;

public sealed class CambiarEstadoRecepcionCommandHandler(
    IComprasDbContext db,
    ICurrentTenant tenant)
    : IRequestHandler<CambiarEstadoRecepcionCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(CambiarEstadoRecepcionCommand request, CancellationToken cancellationToken)
    {
        var recepcion = await db.Recepciones
            .Include(r => r.Lineas)
            .Where(r => r.Id == new RecepcionId(request.RecepcionId) && r.EmpresaId == tenant.EmpresaId)
            .FirstOrDefaultAsync(cancellationToken);

        if (recepcion is null)
            return Result<bool>.Failure(RecepcionErrors.NotFound(request.RecepcionId));

        var result = request.EstadoDestino switch
        {
            EstadoRecepcion.EnInspeccion => recepcion.Inspeccionar(),
            EstadoRecepcion.Aceptada => recepcion.Aceptar(),
            EstadoRecepcion.Rechazada => recepcion.Rechazar(request.Motivo ?? "Sin motivo especificado"),
            _ => Result<bool>.Failure(
                RecepcionErrors.TransicionInvalida(recepcion.EstadoRecepcion.ToString(), request.EstadoDestino.ToString()))
        };

        if (result.IsFailure)
            return result;

        await db.SaveChangesAsync(cancellationToken);
        return Result<bool>.Success(true);
    }
}
