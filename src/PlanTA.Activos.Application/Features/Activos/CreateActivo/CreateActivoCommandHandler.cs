using MediatR;
using Microsoft.EntityFrameworkCore;
using PlanTA.Activos.Application.Interfaces;
using PlanTA.Activos.Domain.Entities;
using PlanTA.Activos.Domain.Errors;
using PlanTA.SharedKernel;

namespace PlanTA.Activos.Application.Features.Activos.CreateActivo;

public sealed class CreateActivoCommandHandler(
    IActivosDbContext db,
    ICurrentTenant tenant)
    : IRequestHandler<CreateActivoCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateActivoCommand request, CancellationToken cancellationToken)
    {
        var codigoUpper = request.Codigo.Trim().ToUpperInvariant();
        var existe = await db.Activos
            .AnyAsync(a => a.Codigo == codigoUpper && a.EmpresaId == tenant.EmpresaId, cancellationToken);

        if (existe)
            return Result<Guid>.Failure(ActivoErrors.CodigoDuplicado(request.Codigo));

        ActivoId? padreId = null;
        if (request.ActivoPadreId.HasValue)
        {
            var padre = await db.Activos
                .AnyAsync(a => a.Id == new ActivoId(request.ActivoPadreId.Value)
                               && a.EmpresaId == tenant.EmpresaId, cancellationToken);
            if (!padre) return Result<Guid>.Failure(ActivoErrors.PadreInvalido);
            padreId = new ActivoId(request.ActivoPadreId.Value);
        }

        var activo = Activo.Crear(
            request.Codigo, request.Nombre, request.Tipo, request.Criticidad,
            tenant.EmpresaId, request.Descripcion, padreId, request.Ubicacion,
            request.Fabricante, request.Modelo, request.NumeroSerie,
            request.FechaAdquisicion, request.CosteAdquisicion);

        db.Activos.Add(activo);
        await db.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(activo.Id.Value);
    }
}
