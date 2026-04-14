using Microsoft.EntityFrameworkCore;
using PlanTA.OEE.Domain.Entities;

namespace PlanTA.OEE.Application.Interfaces;

public interface IOEEDbContext
{
    DbSet<RegistroOEE> Registros { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
