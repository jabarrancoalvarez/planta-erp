using MediatR;
using PlanTA.Costes.Application.Interfaces;
using PlanTA.Costes.Domain.Entities;
using PlanTA.SharedKernel;

namespace PlanTA.Costes.Application.Features.Imputaciones.CreateImputacion;

public sealed class CreateImputacionCommandHandler(
    ICostesDbContext db,
    ICurrentTenant tenant)
    : IRequestHandler<CreateImputacionCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateImputacionCommand request, CancellationToken ct)
    {
        var result = ImputacionCoste.Crear(
            request.Tipo, request.Origen, request.Cantidad, request.PrecioUnitario,
            tenant.EmpresaId, request.OrdenFabricacionId, request.OrdenTrabajoId,
            request.ProductoId, request.Concepto, request.Fecha);

        if (!result.IsSuccess)
            return Result<Guid>.Failure(result.Error!);

        db.Imputaciones.Add(result.Value!);
        await db.SaveChangesAsync(ct);
        return Result<Guid>.Success(result.Value!.Id.Value);
    }
}
