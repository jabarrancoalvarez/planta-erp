using MediatR;
using Microsoft.EntityFrameworkCore;
using PlanTA.OEE.Application.Interfaces;
using PlanTA.OEE.Domain.Entities;
using PlanTA.OEE.Domain.Errors;
using PlanTA.SharedKernel;

namespace PlanTA.OEE.Application.Features.Registros.DeleteRegistroOEE;

public sealed class DeleteRegistroOEECommandHandler(
    IOEEDbContext db,
    ICurrentTenant tenant)
    : IRequestHandler<DeleteRegistroOEECommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(DeleteRegistroOEECommand request, CancellationToken ct)
    {
        var id = new RegistroOEEId(request.Id);
        var registro = await db.Registros
            .FirstOrDefaultAsync(r => r.Id == id && r.EmpresaId == tenant.EmpresaId, ct);

        if (registro is null)
            return Result<bool>.Failure(RegistroOEEErrors.NotFound(request.Id));

        db.Registros.Remove(registro);
        await db.SaveChangesAsync(ct);
        return Result<bool>.Success(true);
    }
}
