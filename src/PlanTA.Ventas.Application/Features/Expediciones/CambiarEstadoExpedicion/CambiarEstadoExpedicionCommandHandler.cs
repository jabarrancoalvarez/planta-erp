using MediatR;
using Microsoft.EntityFrameworkCore;
using PlanTA.Ventas.Application.Interfaces;
using PlanTA.Ventas.Domain.Entities;
using PlanTA.Ventas.Domain.Enums;
using PlanTA.Ventas.Domain.Errors;
using PlanTA.SharedKernel;

namespace PlanTA.Ventas.Application.Features.Expediciones.CambiarEstadoExpedicion;

public sealed class CambiarEstadoExpedicionCommandHandler(
    IVentasDbContext db,
    ICurrentTenant tenant)
    : IRequestHandler<CambiarEstadoExpedicionCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(CambiarEstadoExpedicionCommand request, CancellationToken cancellationToken)
    {
        var expedicion = await db.Expediciones
            .Include(e => e.Lineas)
            .Where(e => e.Id == new ExpedicionId(request.ExpedicionId) && e.EmpresaId == tenant.EmpresaId)
            .FirstOrDefaultAsync(cancellationToken);

        if (expedicion is null)
            return Result<bool>.Failure(ExpedicionErrors.NotFound(request.ExpedicionId));

        var result = request.EstadoDestino switch
        {
            EstadoExpedicion.EnPicking => expedicion.IniciarPicking(),
            EstadoExpedicion.Empaquetada => expedicion.Empaquetar(),
            EstadoExpedicion.Enviada => expedicion.Enviar(),
            EstadoExpedicion.Entregada => expedicion.Entregar(),
            _ => Result<bool>.Failure(
                ExpedicionErrors.TransicionInvalida(expedicion.EstadoExpedicion.ToString(), request.EstadoDestino.ToString()))
        };

        if (result.IsFailure)
            return result;

        await db.SaveChangesAsync(cancellationToken);
        return Result<bool>.Success(true);
    }
}
