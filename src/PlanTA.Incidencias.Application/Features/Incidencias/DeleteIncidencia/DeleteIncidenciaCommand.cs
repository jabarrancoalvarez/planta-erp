using MediatR;
using Microsoft.EntityFrameworkCore;
using PlanTA.Incidencias.Application.Interfaces;
using PlanTA.Incidencias.Domain.Entities;
using PlanTA.Incidencias.Domain.Errors;
using PlanTA.SharedKernel;
using PlanTA.SharedKernel.CQRS;

namespace PlanTA.Incidencias.Application.Features.Incidencias.DeleteIncidencia;

public record DeleteIncidenciaCommand(Guid IncidenciaId) : ICommand<Guid>;

public sealed class DeleteIncidenciaCommandHandler(
    IIncidenciasDbContext db,
    ICurrentTenant tenant)
    : IRequestHandler<DeleteIncidenciaCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(DeleteIncidenciaCommand request, CancellationToken cancellationToken)
    {
        var inc = await db.Incidencias
            .FirstOrDefaultAsync(
                i => i.Id == new IncidenciaId(request.IncidenciaId) && i.EmpresaId == tenant.EmpresaId,
                cancellationToken);

        if (inc is null)
            return Result<Guid>.Failure(IncidenciaErrors.NotFound(request.IncidenciaId));

        inc.SoftDelete();
        await db.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(inc.Id.Value);
    }
}
