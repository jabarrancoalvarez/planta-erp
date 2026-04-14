using Microsoft.EntityFrameworkCore;
using PlanTA.Incidencias.Domain.Entities;

namespace PlanTA.Incidencias.Application.Interfaces;

public interface IIncidenciasDbContext
{
    DbSet<Incidencia> Incidencias { get; }
    DbSet<FotoIncidencia> Fotos { get; }
    DbSet<ComentarioIncidencia> Comentarios { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
