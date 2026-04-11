using MediatR;
using Microsoft.EntityFrameworkCore;
using PlanTA.Calidad.Application.Interfaces;
using PlanTA.Calidad.Domain.Entities;
using PlanTA.Calidad.Domain.Errors;
using PlanTA.SharedKernel;

namespace PlanTA.Calidad.Application.Features.NoConformidades.CreateNC;

public sealed class CreateNCCommandHandler(
    ICalidadDbContext db,
    ICurrentTenant tenant)
    : IRequestHandler<CreateNCCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateNCCommand request, CancellationToken cancellationToken)
    {
        var codigoNormalizado = request.Codigo.Trim().ToUpperInvariant();

        var codigoExists = await db.NoConformidades
            .AnyAsync(nc => nc.Codigo == codigoNormalizado && nc.EmpresaId == tenant.EmpresaId, cancellationToken);

        if (codigoExists)
            return Result<Guid>.Failure(NoConformidadErrors.CodigoDuplicado(request.Codigo));

        InspeccionId? inspeccionId = request.InspeccionId.HasValue
            ? new InspeccionId(request.InspeccionId.Value)
            : null;

        var nc = NoConformidad.Crear(
            request.Codigo,
            request.OrigenInspeccion,
            request.ReferenciaOrigenId,
            request.Descripcion,
            request.SeveridadNC,
            tenant.EmpresaId,
            inspeccionId,
            request.ResponsableUserId);

        db.NoConformidades.Add(nc);
        await db.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(nc.Id.Value);
    }
}
