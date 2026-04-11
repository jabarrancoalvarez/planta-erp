using MediatR;
using Microsoft.EntityFrameworkCore;
using PlanTA.Calidad.Application.Interfaces;
using PlanTA.Calidad.Domain.Entities;
using PlanTA.Calidad.Domain.Enums;
using PlanTA.Calidad.Domain.Errors;
using PlanTA.SharedKernel;

namespace PlanTA.Calidad.Application.Features.NoConformidades.CambiarEstadoNC;

public sealed class CambiarEstadoNCCommandHandler(
    ICalidadDbContext db,
    ICurrentTenant tenant)
    : IRequestHandler<CambiarEstadoNCCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(CambiarEstadoNCCommand request, CancellationToken cancellationToken)
    {
        var nc = await db.NoConformidades
            .Include(n => n.Acciones)
            .Where(n => n.Id == new NoConformidadId(request.NoConformidadId) && n.EmpresaId == tenant.EmpresaId)
            .FirstOrDefaultAsync(cancellationToken);

        if (nc is null)
            return Result<bool>.Failure(NoConformidadErrors.NotFound(request.NoConformidadId));

        var result = request.EstadoDestino switch
        {
            EstadoNoConformidad.EnInvestigacion => nc.Investigar(),
            EstadoNoConformidad.AccionDefinida => nc.DefinirAccion(request.CausaRaiz),
            EstadoNoConformidad.Resuelta => nc.Resolver(),
            EstadoNoConformidad.Cerrada => nc.Cerrar(),
            _ => Result<bool>.Failure(
                NoConformidadErrors.TransicionInvalida(
                    nc.EstadoNoConformidad.ToString(), request.EstadoDestino.ToString()))
        };

        if (result.IsFailure)
            return result;

        await db.SaveChangesAsync(cancellationToken);
        return Result<bool>.Success(true);
    }
}
