using MediatR;
using PlanTA.CRM.Application.Interfaces;
using PlanTA.CRM.Domain.Entities;
using PlanTA.SharedKernel;

namespace PlanTA.CRM.Application.Features.Oportunidades.CreateOportunidad;

public sealed class CreateOportunidadCommandHandler(
    ICrmDbContext db,
    ICurrentTenant tenant)
    : IRequestHandler<CreateOportunidadCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateOportunidadCommand request, CancellationToken ct)
    {
        var result = Oportunidad.Crear(
            request.Titulo, request.ImporteEstimado, tenant.EmpresaId,
            request.ClienteId, request.LeadId, request.FechaCierreEstimada,
            request.Descripcion, request.PropietarioUserId);

        if (!result.IsSuccess) return Result<Guid>.Failure(result.Error!);

        db.Oportunidades.Add(result.Value!);
        await db.SaveChangesAsync(ct);
        return Result<Guid>.Success(result.Value!.Id.Value);
    }
}
