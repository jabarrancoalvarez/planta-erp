using MediatR;
using Microsoft.EntityFrameworkCore;
using PlanTA.Produccion.Application.DTOs;
using PlanTA.Produccion.Application.Interfaces;
using PlanTA.Produccion.Domain.Entities;
using PlanTA.Produccion.Domain.Errors;
using PlanTA.SharedKernel;

namespace PlanTA.Produccion.Application.Features.OrdenesFabricacion.GetOF;

public sealed class GetOFQueryHandler(
    IProduccionDbContext db,
    ICurrentTenant tenant)
    : IRequestHandler<GetOFQuery, Result<OrdenFabricacionDetailDto>>
{
    public async Task<Result<OrdenFabricacionDetailDto>> Handle(GetOFQuery request, CancellationToken cancellationToken)
    {
        var of = await db.OrdenesFabricacion
            .AsNoTracking()
            .Include(o => o.LineasConsumo)
            .Include(o => o.PartesProduccion)
            .Where(o => o.Id == new OrdenFabricacionId(request.OrdenFabricacionId) && o.EmpresaId == tenant.EmpresaId)
            .FirstOrDefaultAsync(cancellationToken);

        if (of is null)
            return Result<OrdenFabricacionDetailDto>.Failure(
                OrdenFabricacionErrors.NotFound(request.OrdenFabricacionId));

        var dto = new OrdenFabricacionDetailDto(
            of.Id.Value,
            of.CodigoOF.Value,
            of.ProductoId,
            of.ListaMaterialesId.Value,
            of.RutaProduccionId?.Value,
            of.CantidadPlanificada.Cantidad,
            of.CantidadPlanificada.UnidadMedida,
            of.EstadoOF,
            of.FechaInicio,
            of.FechaFin,
            of.Prioridad,
            of.Observaciones,
            of.CreatedAt,
            of.LineasConsumo.Select(l => new LineaConsumoOFDto(
                l.Id.Value, l.ProductoId, l.LoteId, l.Cantidad, l.FechaConsumo)).ToList(),
            of.PartesProduccion.Select(p => new ParteProduccionDto(
                p.Id.Value, p.FechaRegistro, p.UnidadesBuenas, p.UnidadesDefectuosas,
                p.Merma, p.LoteGeneradoId, p.Observaciones)).ToList());

        return Result<OrdenFabricacionDetailDto>.Success(dto);
    }
}
