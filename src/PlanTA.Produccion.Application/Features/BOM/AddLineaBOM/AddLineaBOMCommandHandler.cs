using MediatR;
using Microsoft.EntityFrameworkCore;
using PlanTA.Produccion.Application.Interfaces;
using PlanTA.Produccion.Domain.Entities;
using PlanTA.Produccion.Domain.Errors;
using PlanTA.SharedKernel;

namespace PlanTA.Produccion.Application.Features.BOM.AddLineaBOM;

public sealed class AddLineaBOMCommandHandler(
    IProduccionDbContext db,
    ICurrentTenant tenant)
    : IRequestHandler<AddLineaBOMCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(AddLineaBOMCommand request, CancellationToken cancellationToken)
    {
        var bom = await db.ListasMateriales
            .Include(b => b.Lineas)
            .Where(b => b.Id == new ListaMaterialesId(request.ListaMaterialesId) && b.EmpresaId == tenant.EmpresaId)
            .FirstOrDefaultAsync(cancellationToken);

        if (bom is null)
            return Result<Guid>.Failure(ListaMaterialesErrors.NotFound(request.ListaMaterialesId));

        var existeComponente = bom.Lineas.Any(l => l.ComponenteProductoId == request.ComponenteProductoId);
        if (existeComponente)
            return Result<Guid>.Failure(ListaMaterialesErrors.LineaComponenteDuplicado(request.ComponenteProductoId));

        var linea = bom.AgregarLinea(
            request.ComponenteProductoId,
            request.Cantidad,
            request.UnidadMedida,
            request.Merma,
            request.Orden);

        await db.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(linea.Id.Value);
    }
}
