using Microsoft.EntityFrameworkCore;
using PlanTA.Costes.Domain.Entities;

namespace PlanTA.Costes.Application.Interfaces;

public interface ICostesDbContext
{
    DbSet<ImputacionCoste> Imputaciones { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
