using MediatR;
using Microsoft.EntityFrameworkCore;
using PlanTA.Calidad.Application.DTOs;
using PlanTA.Calidad.Application.Interfaces;
using PlanTA.Calidad.Domain.Entities;
using PlanTA.Calidad.Domain.Errors;
using PlanTA.SharedKernel;

namespace PlanTA.Calidad.Application.Features.NoConformidades.GetNC;

public sealed class GetNCQueryHandler(
    ICalidadDbContext db,
    ICurrentTenant tenant)
    : IRequestHandler<GetNCQuery, Result<NCDetailDto>>
{
    public async Task<Result<NCDetailDto>> Handle(GetNCQuery request, CancellationToken cancellationToken)
    {
        var nc = await db.NoConformidades
            .AsNoTracking()
            .Include(n => n.Acciones)
            .Where(n => n.Id == new NoConformidadId(request.NoConformidadId) && n.EmpresaId == tenant.EmpresaId)
            .FirstOrDefaultAsync(cancellationToken);

        if (nc is null)
            return Result<NCDetailDto>.Failure(NoConformidadErrors.NotFound(request.NoConformidadId));

        var dto = new NCDetailDto(
            nc.Id.Value,
            nc.Codigo,
            nc.InspeccionId?.Value,
            nc.OrigenInspeccion,
            nc.ReferenciaOrigenId,
            nc.Descripcion,
            nc.SeveridadNC,
            nc.EstadoNoConformidad,
            nc.CausaRaiz,
            nc.FechaDeteccion,
            nc.FechaCierre,
            nc.ResponsableUserId,
            nc.CreatedAt,
            nc.Acciones.Select(a => new AccionCorrectivaDto(
                a.Id.Value, a.Descripcion, a.ResponsableUserId,
                a.FechaLimite, a.FechaCompletada, a.Completada)).ToList());

        return Result<NCDetailDto>.Success(dto);
    }
}
