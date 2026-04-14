using MediatR;
using Microsoft.EntityFrameworkCore;
using PlanTA.Produccion.Application.Interfaces;
using PlanTA.Produccion.Domain.Entities;
using PlanTA.Produccion.Domain.Errors;
using PlanTA.SharedKernel;
using PlanTA.SharedKernel.CQRS;

namespace PlanTA.Produccion.Application.Features.BOM.UpdateBOM;

public record UpdateBOMCommand(Guid BOMId, string Nombre, string? Descripcion) : ICommand<Guid>;

public sealed class UpdateBOMCommandHandler(
    IProduccionDbContext db,
    ICurrentTenant tenant)
    : IRequestHandler<UpdateBOMCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(UpdateBOMCommand request, CancellationToken cancellationToken)
    {
        var bom = await db.ListasMateriales
            .FirstOrDefaultAsync(
                b => b.Id == new ListaMaterialesId(request.BOMId) && b.EmpresaId == tenant.EmpresaId,
                cancellationToken);

        if (bom is null)
            return Result<Guid>.Failure(ListaMaterialesErrors.NotFound(request.BOMId));

        bom.Actualizar(request.Nombre, request.Descripcion);
        await db.SaveChangesAsync(cancellationToken);
        return Result<Guid>.Success(bom.Id.Value);
    }
}
