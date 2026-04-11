using MediatR;
using Microsoft.EntityFrameworkCore;
using PlanTA.Produccion.Application.Interfaces;
using PlanTA.Produccion.Domain.Entities;
using PlanTA.Produccion.Domain.Errors;
using PlanTA.SharedKernel;

namespace PlanTA.Produccion.Application.Features.OrdenesFabricacion.CreateOF;

public sealed class CreateOFCommandHandler(
    IProduccionDbContext db,
    ICurrentTenant tenant)
    : IRequestHandler<CreateOFCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateOFCommand request, CancellationToken cancellationToken)
    {
        var codigoExiste = await db.OrdenesFabricacion
            .AnyAsync(o => o.CodigoOF.Value == request.CodigoOF.Trim().ToUpperInvariant()
                           && o.EmpresaId == tenant.EmpresaId, cancellationToken);

        if (codigoExiste)
            return Result<Guid>.Failure(OrdenFabricacionErrors.CodigoDuplicado(request.CodigoOF));

        var bomExiste = await db.ListasMateriales
            .AnyAsync(b => b.Id == new ListaMaterialesId(request.ListaMaterialesId)
                           && b.EmpresaId == tenant.EmpresaId, cancellationToken);

        if (!bomExiste)
            return Result<Guid>.Failure(ListaMaterialesErrors.NotFound(request.ListaMaterialesId));

        var of = OrdenFabricacion.Crear(
            request.CodigoOF,
            request.ProductoId,
            new ListaMaterialesId(request.ListaMaterialesId),
            request.CantidadPlanificada,
            request.UnidadMedida,
            tenant.EmpresaId,
            request.RutaProduccionId.HasValue ? new RutaProduccionId(request.RutaProduccionId.Value) : null,
            request.Prioridad,
            request.Observaciones);

        db.OrdenesFabricacion.Add(of);
        await db.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(of.Id.Value);
    }
}
