using MediatR;
using Microsoft.EntityFrameworkCore;
using PlanTA.Mantenimiento.Application.Interfaces;
using PlanTA.Mantenimiento.Domain.Entities;
using PlanTA.Mantenimiento.Domain.Enums;
using PlanTA.Mantenimiento.Domain.Errors;
using PlanTA.SharedKernel;
using PlanTA.SharedKernel.CQRS;

namespace PlanTA.Mantenimiento.Application.Features.Planes.CreatePlan;

public record CreatePlanCommand(
    string Codigo,
    string Nombre,
    Guid ActivoId,
    FrecuenciaPlan Frecuencia,
    int Intervalo,
    string? Descripcion = null,
    decimal HorasEstimadas = 0,
    decimal? UmbralHorasUso = null,
    DateTimeOffset? ProximaEjecucion = null) : ICommand<Guid>;

public sealed class CreatePlanCommandHandler(
    IMantenimientoDbContext db,
    ICurrentTenant tenant)
    : IRequestHandler<CreatePlanCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreatePlanCommand request, CancellationToken cancellationToken)
    {
        var codigo = request.Codigo.Trim().ToUpperInvariant();
        var existe = await db.Planes
            .AnyAsync(p => p.Codigo == codigo && p.EmpresaId == tenant.EmpresaId, cancellationToken);
        if (existe)
            return Result<Guid>.Failure(PlanMantenimientoErrors.CodigoDuplicado(request.Codigo));

        var plan = PlanMantenimiento.Crear(
            request.Codigo, request.Nombre, request.ActivoId, request.Frecuencia,
            request.Intervalo, tenant.EmpresaId, request.Descripcion,
            request.HorasEstimadas, request.UmbralHorasUso, request.ProximaEjecucion);

        db.Planes.Add(plan);
        await db.SaveChangesAsync(cancellationToken);
        return Result<Guid>.Success(plan.Id.Value);
    }
}
