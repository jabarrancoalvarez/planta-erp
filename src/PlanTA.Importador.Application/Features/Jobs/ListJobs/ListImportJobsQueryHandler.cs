using MediatR;
using Microsoft.EntityFrameworkCore;
using PlanTA.Importador.Application.DTOs;
using PlanTA.Importador.Application.Interfaces;
using PlanTA.SharedKernel;

namespace PlanTA.Importador.Application.Features.Jobs.ListJobs;

public sealed class ListImportJobsQueryHandler(
    IImportadorDbContext db,
    ICurrentTenant tenant)
    : IRequestHandler<ListImportJobsQuery, Result<PagedResult<ImportJobDto>>>
{
    public async Task<Result<PagedResult<ImportJobDto>>> Handle(
        ListImportJobsQuery request, CancellationToken ct)
    {
        var query = db.Jobs.AsNoTracking()
            .Where(j => j.EmpresaId == tenant.EmpresaId);

        var total = await query.CountAsync(ct);

        var items = await query
            .OrderByDescending(j => j.CreatedAt)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(j => new ImportJobDto(
                j.Id.Value, j.Tipo, j.Formato, j.Estado,
                j.NombreArchivo, j.FilasTotales, j.FilasValidas, j.FilasConError,
                j.FilasImportadas, j.ResumenErrores, j.IniciadoEn, j.FinalizadoEn))
            .ToListAsync(ct);

        return Result<PagedResult<ImportJobDto>>.Success(
            new PagedResult<ImportJobDto>(items, total, request.Page, request.PageSize));
    }
}
