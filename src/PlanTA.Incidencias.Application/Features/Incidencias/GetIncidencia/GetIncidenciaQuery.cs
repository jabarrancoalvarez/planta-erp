using MediatR;
using Microsoft.EntityFrameworkCore;
using PlanTA.Incidencias.Application.DTOs;
using PlanTA.Incidencias.Application.Interfaces;
using PlanTA.Incidencias.Domain.Entities;
using PlanTA.Incidencias.Domain.Errors;
using PlanTA.SharedKernel;
using PlanTA.SharedKernel.CQRS;

namespace PlanTA.Incidencias.Application.Features.Incidencias.GetIncidencia;

public record GetIncidenciaQuery(Guid IncidenciaId) : IQuery<IncidenciaDto>;

public sealed class GetIncidenciaQueryHandler(
    IIncidenciasDbContext db,
    ICurrentTenant tenant)
    : IRequestHandler<GetIncidenciaQuery, Result<IncidenciaDto>>
{
    public async Task<Result<IncidenciaDto>> Handle(GetIncidenciaQuery request, CancellationToken cancellationToken)
    {
        var inc = await db.Incidencias
            .Where(i => i.Id == new IncidenciaId(request.IncidenciaId) && i.EmpresaId == tenant.EmpresaId)
            .Select(i => new IncidenciaDto(
                i.Id.Value, i.Codigo, i.Titulo, i.Descripcion, i.Tipo, i.Severidad, i.Estado,
                i.ActivoId, i.UbicacionTexto, i.ReportadoPorUserId, i.AsignadoAUserId,
                i.FechaApertura, i.FechaCierre, i.OrdenTrabajoId, i.CausaRaiz, i.ResolucionNotas,
                i.ParadaLinea))
            .FirstOrDefaultAsync(cancellationToken);

        return inc is null
            ? Result<IncidenciaDto>.Failure(IncidenciaErrors.NotFound(request.IncidenciaId))
            : Result<IncidenciaDto>.Success(inc);
    }
}
