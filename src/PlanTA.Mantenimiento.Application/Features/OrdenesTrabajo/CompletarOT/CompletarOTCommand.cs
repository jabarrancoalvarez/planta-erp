using MediatR;
using Microsoft.EntityFrameworkCore;
using PlanTA.Mantenimiento.Application.Interfaces;
using PlanTA.Mantenimiento.Domain.Entities;
using PlanTA.Mantenimiento.Domain.Errors;
using PlanTA.SharedKernel;
using PlanTA.SharedKernel.CQRS;

namespace PlanTA.Mantenimiento.Application.Features.OrdenesTrabajo.CompletarOT;

public record CompletarOTCommand(
    Guid Id,
    decimal HorasReales,
    decimal CosteManoObra,
    string? NotasCierre) : ICommand<Guid>;

public sealed class CompletarOTCommandHandler(
    IMantenimientoDbContext db,
    ICurrentTenant tenant)
    : IRequestHandler<CompletarOTCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CompletarOTCommand request, CancellationToken cancellationToken)
    {
        var id = new OrdenTrabajoId(request.Id);
        var ot = await db.OrdenesTrabajo
            .FirstOrDefaultAsync(o => o.Id == id && o.EmpresaId == tenant.EmpresaId, cancellationToken);

        if (ot is null)
            return Result<Guid>.Failure(OrdenTrabajoErrors.NotFound(request.Id));

        var res = ot.Completar(request.HorasReales, request.CosteManoObra, request.NotasCierre);
        if (res.IsFailure) return Result<Guid>.Failure(res.Error!);

        await db.SaveChangesAsync(cancellationToken);
        return Result<Guid>.Success(ot.Id.Value);
    }
}
