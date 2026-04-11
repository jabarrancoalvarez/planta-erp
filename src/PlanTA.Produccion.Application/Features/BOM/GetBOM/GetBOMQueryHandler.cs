using MediatR;
using Microsoft.EntityFrameworkCore;
using PlanTA.Produccion.Application.DTOs;
using PlanTA.Produccion.Application.Interfaces;
using PlanTA.Produccion.Domain.Entities;
using PlanTA.Produccion.Domain.Errors;
using PlanTA.SharedKernel;

namespace PlanTA.Produccion.Application.Features.BOM.GetBOM;

public sealed class GetBOMQueryHandler(
    IProduccionDbContext db,
    ICurrentTenant tenant)
    : IRequestHandler<GetBOMQuery, Result<ListaMaterialesDetailDto>>
{
    public async Task<Result<ListaMaterialesDetailDto>> Handle(GetBOMQuery request, CancellationToken cancellationToken)
    {
        var bom = await db.ListasMateriales
            .AsNoTracking()
            .Include(b => b.Lineas)
            .Where(b => b.Id == new ListaMaterialesId(request.ListaMaterialesId) && b.EmpresaId == tenant.EmpresaId)
            .FirstOrDefaultAsync(cancellationToken);

        if (bom is null)
            return Result<ListaMaterialesDetailDto>.Failure(ListaMaterialesErrors.NotFound(request.ListaMaterialesId));

        var dto = new ListaMaterialesDetailDto(
            bom.Id.Value,
            bom.ProductoId,
            bom.Nombre,
            bom.Descripcion,
            bom.VersionBOM,
            bom.Activo,
            bom.CreatedAt,
            bom.Lineas.Select(l => new LineaBOMDto(
                l.Id.Value,
                l.ComponenteProductoId,
                l.Cantidad,
                l.UnidadMedida,
                l.Merma,
                l.Orden)).OrderBy(l => l.Orden).ToList());

        return Result<ListaMaterialesDetailDto>.Success(dto);
    }
}
