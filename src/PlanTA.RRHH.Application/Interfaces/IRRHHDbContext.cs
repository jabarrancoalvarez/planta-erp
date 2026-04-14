using Microsoft.EntityFrameworkCore;
using PlanTA.RRHH.Domain.Entities;

namespace PlanTA.RRHH.Application.Interfaces;

public interface IRRHHDbContext
{
    DbSet<Empleado> Empleados { get; }
    DbSet<Turno> Turnos { get; }
    DbSet<Fichaje> Fichajes { get; }
    DbSet<Ausencia> Ausencias { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
