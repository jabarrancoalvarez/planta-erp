using MediatR;
using Microsoft.EntityFrameworkCore;
using PlanTA.Calidad.Application.Interfaces;
using PlanTA.Calidad.Domain.Entities;
using PlanTA.Calidad.Domain.Errors;
using PlanTA.SharedKernel;

namespace PlanTA.Calidad.Application.Features.NoConformidades.AddAccionCorrectiva;

public sealed class AddAccionCorrectivaCommandHandler(
    ICalidadDbContext db,
    ICurrentTenant tenant)
    : IRequestHandler<AddAccionCorrectivaCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(AddAccionCorrectivaCommand request, CancellationToken cancellationToken)
    {
        var nc = await db.NoConformidades
            .Include(n => n.Acciones)
            .Where(n => n.Id == new NoConformidadId(request.NoConformidadId) && n.EmpresaId == tenant.EmpresaId)
            .FirstOrDefaultAsync(cancellationToken);

        if (nc is null)
            return Result<Guid>.Failure(NoConformidadErrors.NotFound(request.NoConformidadId));

        var accion = nc.AgregarAccion(
            request.Descripcion, request.ResponsableUserId, request.FechaLimite);

        await db.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(accion.Id.Value);
    }
}
