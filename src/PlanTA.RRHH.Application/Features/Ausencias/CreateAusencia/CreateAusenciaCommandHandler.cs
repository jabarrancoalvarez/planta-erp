using MediatR;
using PlanTA.RRHH.Application.Interfaces;
using PlanTA.RRHH.Domain.Entities;
using PlanTA.SharedKernel;

namespace PlanTA.RRHH.Application.Features.Ausencias.CreateAusencia;

public sealed class CreateAusenciaCommandHandler(
    IRRHHDbContext db,
    ICurrentTenant tenant)
    : IRequestHandler<CreateAusenciaCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateAusenciaCommand request, CancellationToken ct)
    {
        var result = Ausencia.Crear(
            new EmpleadoId(request.EmpleadoId), request.Tipo,
            request.FechaInicio, request.FechaFin, tenant.EmpresaId, request.Motivo);

        if (!result.IsSuccess)
            return Result<Guid>.Failure(result.Error!);

        db.Ausencias.Add(result.Value!);
        await db.SaveChangesAsync(ct);
        return Result<Guid>.Success(result.Value!.Id.Value);
    }
}
