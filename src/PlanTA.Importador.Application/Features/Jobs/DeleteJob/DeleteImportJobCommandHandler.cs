using MediatR;
using Microsoft.EntityFrameworkCore;
using PlanTA.Importador.Application.Interfaces;
using PlanTA.Importador.Domain.Entities;
using PlanTA.Importador.Domain.Errors;
using PlanTA.SharedKernel;

namespace PlanTA.Importador.Application.Features.Jobs.DeleteJob;

public sealed class DeleteImportJobCommandHandler(
    IImportadorDbContext db,
    ICurrentTenant tenant)
    : IRequestHandler<DeleteImportJobCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(DeleteImportJobCommand request, CancellationToken ct)
    {
        var id = new ImportJobId(request.Id);
        var job = await db.Jobs
            .FirstOrDefaultAsync(j => j.Id == id && j.EmpresaId == tenant.EmpresaId, ct);

        if (job is null)
            return Result<bool>.Failure(ImportJobErrors.NotFound(request.Id));

        db.Jobs.Remove(job);
        await db.SaveChangesAsync(ct);
        return Result<bool>.Success(true);
    }
}
