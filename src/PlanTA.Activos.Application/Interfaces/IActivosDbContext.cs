using Microsoft.EntityFrameworkCore;
using PlanTA.Activos.Domain.Entities;

namespace PlanTA.Activos.Application.Interfaces;

public interface IActivosDbContext
{
    DbSet<Activo> Activos { get; }
    DbSet<DocumentoActivo> Documentos { get; }
    DbSet<LecturaActivo> Lecturas { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
