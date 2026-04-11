using MediatR;
using Microsoft.EntityFrameworkCore;
using PlanTA.Calidad.Application.DTOs;
using PlanTA.Calidad.Application.Interfaces;
using PlanTA.Calidad.Domain.Entities;
using PlanTA.Calidad.Domain.Errors;
using PlanTA.SharedKernel;

namespace PlanTA.Calidad.Application.Features.Inspecciones.GetInspeccion;

public sealed class GetInspeccionQueryHandler(
    ICalidadDbContext db,
    ICurrentTenant tenant)
    : IRequestHandler<GetInspeccionQuery, Result<InspeccionDetailDto>>
{
    public async Task<Result<InspeccionDetailDto>> Handle(
        GetInspeccionQuery request, CancellationToken cancellationToken)
    {
        var inspeccion = await db.Inspecciones
            .AsNoTracking()
            .Include(i => i.Resultados)
            .Where(i => i.Id == new InspeccionId(request.InspeccionId) && i.EmpresaId == tenant.EmpresaId)
            .FirstOrDefaultAsync(cancellationToken);

        if (inspeccion is null)
            return Result<InspeccionDetailDto>.Failure(InspeccionErrors.NotFound(request.InspeccionId));

        var dto = new InspeccionDetailDto(
            inspeccion.Id.Value,
            inspeccion.PlantillaInspeccionId.Value,
            inspeccion.OrigenInspeccion,
            inspeccion.ReferenciaOrigenId,
            inspeccion.LoteId,
            inspeccion.FechaInspeccion,
            inspeccion.InspectorUserId,
            inspeccion.ResultadoInspeccion,
            inspeccion.Observaciones,
            inspeccion.CreatedAt,
            inspeccion.Resultados.Select(r => new ResultadoCriterioDto(
                r.Id.Value, r.CriterioInspeccionId.Value, r.NombreCriterio,
                r.EsObligatorio, r.ValorMedido, r.Cumple, r.Observaciones)).ToList());

        return Result<InspeccionDetailDto>.Success(dto);
    }
}
