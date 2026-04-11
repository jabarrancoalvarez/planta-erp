using MediatR;
using PlanTA.Produccion.Application.Interfaces;
using PlanTA.Produccion.Domain.Entities;
using PlanTA.SharedKernel;

namespace PlanTA.Produccion.Application.Features.BOM.CreateBOM;

public sealed class CreateBOMCommandHandler(
    IProduccionDbContext db,
    ICurrentTenant tenant)
    : IRequestHandler<CreateBOMCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateBOMCommand request, CancellationToken cancellationToken)
    {
        var bom = ListaMateriales.Crear(
            request.ProductoId,
            request.Nombre,
            tenant.EmpresaId,
            request.Descripcion);

        db.ListasMateriales.Add(bom);
        await db.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(bom.Id.Value);
    }
}
