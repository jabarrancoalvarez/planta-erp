using Microsoft.EntityFrameworkCore;
using PlanTA.Mantenimiento.Domain.Entities;

namespace PlanTA.Mantenimiento.Application.Interfaces;

public interface IMantenimientoDbContext
{
    DbSet<OrdenTrabajo> OrdenesTrabajo { get; }
    DbSet<TareaOT> Tareas { get; }
    DbSet<RepuestoOT> Repuestos { get; }
    DbSet<PlanMantenimiento> Planes { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
