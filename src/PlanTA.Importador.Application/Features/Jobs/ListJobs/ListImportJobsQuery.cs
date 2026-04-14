using PlanTA.Importador.Application.DTOs;
using PlanTA.SharedKernel.CQRS;

namespace PlanTA.Importador.Application.Features.Jobs.ListJobs;

public record ListImportJobsQuery(int Page = 1, int PageSize = 20)
    : IQuery<PagedResult<ImportJobDto>>;
