using MediatR;
using Microsoft.EntityFrameworkCore;
using PlanTA.Produccion.Application.Interfaces;
using PlanTA.Produccion.Domain.Entities;
using PlanTA.Produccion.Domain.Errors;
using PlanTA.SharedKernel;

namespace PlanTA.Produccion.Application.Features.BOM.DeleteBOM;

public sealed class DeleteBOMCommandHandler(
    IProduccionDbContext db,
    ICurrentTenant tenant)
    : IRequestHandler<DeleteBOMCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(DeleteBOMCommand request, CancellationToken cancellationToken)
    {
        var bom = await db.ListasMateriales
            .FirstOrDefaultAsync(
                b => b.Id == new ListaMaterialesId(request.ListaMaterialesId) && b.EmpresaId == tenant.EmpresaId,
                cancellationToken);

        if (bom is null)
            return Result<Guid>.Failure(ListaMaterialesErrors.NotFound(request.ListaMaterialesId));

        bom.SoftDelete();
        await db.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(bom.Id.Value);
    }
}
