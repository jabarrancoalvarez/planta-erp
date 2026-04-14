using MediatR;
using Microsoft.EntityFrameworkCore;
using PlanTA.Incidencias.Application.Interfaces;
using PlanTA.Incidencias.Domain.Entities;
using PlanTA.Incidencias.Domain.Errors;
using PlanTA.SharedKernel;
using PlanTA.SharedKernel.CQRS;

namespace PlanTA.Incidencias.Application.Features.Incidencias.CerrarIncidencia;

public record CerrarIncidenciaCommand(Guid Id, string? CausaRaiz, string? ResolucionNotas) : ICommand<Guid>;

public sealed class CerrarIncidenciaCommandHandler(
    IIncidenciasDbContext db,
    ICurrentTenant tenant)
    : IRequestHandler<CerrarIncidenciaCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CerrarIncidenciaCommand request, CancellationToken cancellationToken)
    {
        var id = new IncidenciaId(request.Id);
        var inc = await db.Incidencias
            .FirstOrDefaultAsync(i => i.Id == id && i.EmpresaId == tenant.EmpresaId, cancellationToken);

        if (inc is null)
            return Result<Guid>.Failure(IncidenciaErrors.NotFound(request.Id));

        var res = inc.Cerrar(request.CausaRaiz, request.ResolucionNotas);
        if (res.IsFailure) return Result<Guid>.Failure(res.Error!);

        await db.SaveChangesAsync(cancellationToken);
        return Result<Guid>.Success(inc.Id.Value);
    }
}
