using Microsoft.EntityFrameworkCore;
using PlanTA.Importador.Domain.Entities;

namespace PlanTA.Importador.Application.Interfaces;

public interface IImportadorDbContext
{
    DbSet<ImportJob> Jobs { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
