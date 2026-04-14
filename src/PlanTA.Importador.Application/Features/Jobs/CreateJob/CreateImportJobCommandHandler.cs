using MediatR;
using PlanTA.Importador.Application.Interfaces;
using PlanTA.Importador.Domain.Entities;
using PlanTA.SharedKernel;

namespace PlanTA.Importador.Application.Features.Jobs.CreateJob;

public sealed class CreateImportJobCommandHandler(
    IImportadorDbContext db,
    ICurrentTenant tenant)
    : IRequestHandler<CreateImportJobCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateImportJobCommand request, CancellationToken ct)
    {
        var job = ImportJob.Crear(
            request.Tipo, request.Formato, request.NombreArchivo,
            request.UserId, tenant.EmpresaId);

        db.Jobs.Add(job);
        await db.SaveChangesAsync(ct);
        return Result<Guid>.Success(job.Id.Value);
    }
}
