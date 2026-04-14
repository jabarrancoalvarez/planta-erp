using MediatR;
using Microsoft.EntityFrameworkCore;
using PlanTA.Costes.Application.DTOs;
using PlanTA.Costes.Application.Interfaces;
using PlanTA.Costes.Domain.Enums;
using PlanTA.SharedKernel;

namespace PlanTA.Costes.Application.Features.Imputaciones.ResumenOF;

public sealed class ResumenOFQueryHandler(
    ICostesDbContext db,
    ICurrentTenant tenant)
    : IRequestHandler<ResumenOFQuery, Result<ResumenCosteOFDto>>
{
    public async Task<Result<ResumenCosteOFDto>> Handle(ResumenOFQuery request, CancellationToken ct)
    {
        var rows = await db.Imputaciones.AsNoTracking()
            .Where(i => i.EmpresaId == tenant.EmpresaId
                     && i.OrdenFabricacionId == request.OrdenFabricacionId)
            .Select(i => new { i.Tipo, i.Importe })
            .ToListAsync(ct);

        var mat = rows.Where(r => r.Tipo == TipoCoste.Material).Sum(r => r.Importe);
        var mo = rows.Where(r => r.Tipo == TipoCoste.ManoObra).Sum(r => r.Importe);
        var maq = rows.Where(r => r.Tipo == TipoCoste.Maquina).Sum(r => r.Importe);
        var otros = rows.Where(r => r.Tipo != TipoCoste.Material
                                 && r.Tipo != TipoCoste.ManoObra
                                 && r.Tipo != TipoCoste.Maquina).Sum(r => r.Importe);

        return Result<ResumenCosteOFDto>.Success(new ResumenCosteOFDto(
            request.OrdenFabricacionId, mat, mo, maq, otros,
            mat + mo + maq + otros, rows.Count));
    }
}
