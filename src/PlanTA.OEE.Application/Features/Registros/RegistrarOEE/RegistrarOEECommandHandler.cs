using MediatR;
using PlanTA.OEE.Application.Interfaces;
using PlanTA.OEE.Domain.Entities;
using PlanTA.SharedKernel;

namespace PlanTA.OEE.Application.Features.Registros.RegistrarOEE;

public sealed class RegistrarOEECommandHandler(
    IOEEDbContext db,
    ICurrentTenant tenant)
    : IRequestHandler<RegistrarOEECommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(RegistrarOEECommand request, CancellationToken ct)
    {
        var result = RegistroOEE.Crear(
            request.ActivoId, request.Fecha,
            request.MinutosPlanificados, request.MinutosFuncionamiento,
            request.PiezasTotales, request.PiezasBuenas, request.TiempoCicloTeoricoSeg,
            tenant.EmpresaId, request.TurnoId, request.OrdenFabricacionId, request.Notas);

        if (!result.IsSuccess) return Result<Guid>.Failure(result.Error!);

        db.Registros.Add(result.Value!);
        await db.SaveChangesAsync(ct);
        return Result<Guid>.Success(result.Value!.Id.Value);
    }
}
