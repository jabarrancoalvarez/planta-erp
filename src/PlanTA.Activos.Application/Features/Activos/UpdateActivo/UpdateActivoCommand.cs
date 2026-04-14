using MediatR;
using Microsoft.EntityFrameworkCore;
using PlanTA.Activos.Application.Interfaces;
using PlanTA.Activos.Domain.Entities;
using PlanTA.Activos.Domain.Enums;
using PlanTA.Activos.Domain.Errors;
using PlanTA.SharedKernel;
using PlanTA.SharedKernel.CQRS;

namespace PlanTA.Activos.Application.Features.Activos.UpdateActivo;

public record UpdateActivoCommand(
    Guid ActivoId,
    string Nombre,
    CriticidadActivo Criticidad,
    string? Descripcion = null,
    string? Ubicacion = null,
    string? Fabricante = null,
    string? Modelo = null) : ICommand<Guid>;

public sealed class UpdateActivoCommandHandler(
    IActivosDbContext db,
    ICurrentTenant tenant)
    : IRequestHandler<UpdateActivoCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(UpdateActivoCommand request, CancellationToken cancellationToken)
    {
        var activo = await db.Activos
            .FirstOrDefaultAsync(
                a => a.Id == new ActivoId(request.ActivoId) && a.EmpresaId == tenant.EmpresaId,
                cancellationToken);

        if (activo is null)
            return Result<Guid>.Failure(ActivoErrors.NotFound(request.ActivoId));

        activo.Actualizar(
            request.Nombre, request.Descripcion, request.Criticidad,
            request.Ubicacion, request.Fabricante, request.Modelo);

        await db.SaveChangesAsync(cancellationToken);
        return Result<Guid>.Success(activo.Id.Value);
    }
}
