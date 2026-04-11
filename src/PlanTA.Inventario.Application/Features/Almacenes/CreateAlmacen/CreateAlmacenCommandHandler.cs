using MediatR;
using PlanTA.Inventario.Application.Interfaces;
using PlanTA.Inventario.Domain.Entities;
using PlanTA.SharedKernel;

namespace PlanTA.Inventario.Application.Features.Almacenes.CreateAlmacen;

public sealed class CreateAlmacenCommandHandler(
    IInventarioDbContext db,
    ICurrentTenant tenant)
    : IRequestHandler<CreateAlmacenCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateAlmacenCommand request, CancellationToken cancellationToken)
    {
        var almacen = Almacen.Crear(
            request.Nombre, tenant.EmpresaId,
            request.Direccion, request.Descripcion, request.EsPrincipal);

        db.Almacenes.Add(almacen);
        await db.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(almacen.Id.Value);
    }
}
