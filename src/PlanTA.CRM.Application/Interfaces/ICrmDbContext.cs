using Microsoft.EntityFrameworkCore;
using PlanTA.CRM.Domain.Entities;

namespace PlanTA.CRM.Application.Interfaces;

public interface ICrmDbContext
{
    DbSet<Lead> Leads { get; }
    DbSet<Oportunidad> Oportunidades { get; }
    DbSet<ActividadCrm> Actividades { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
