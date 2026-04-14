using MediatR;
using Microsoft.EntityFrameworkCore;
using PlanTA.Activos.Application.Interfaces;
using PlanTA.Activos.Domain.Entities;
using PlanTA.Activos.Domain.Enums;
using PlanTA.Activos.Domain.Errors;
using PlanTA.SharedKernel;
using PlanTA.SharedKernel.CQRS;

namespace PlanTA.Activos.Application.Features.Activos.CambiarEstado;

public record CambiarEstadoActivoCommand(Guid Id, EstadoActivo NuevoEstado) : ICommand<Guid>;

public sealed class CambiarEstadoActivoCommandHandler(
    IActivosDbContext db,
    ICurrentTenant tenant)
    : IRequestHandler<CambiarEstadoActivoCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CambiarEstadoActivoCommand request, CancellationToken cancellationToken)
    {
        var id = new ActivoId(request.Id);
        var activo = await db.Activos
            .FirstOrDefaultAsync(a => a.Id == id && a.EmpresaId == tenant.EmpresaId, cancellationToken);

        if (activo is null)
            return Result<Guid>.Failure(ActivoErrors.NotFound(request.Id));

        activo.CambiarEstado(request.NuevoEstado);
        await db.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(activo.Id.Value);
    }
}
