using Microsoft.EntityFrameworkCore;
using PlanTA.IA.Domain.Entities;

namespace PlanTA.IA.Application.Interfaces;

public interface IIADbContext
{
    DbSet<ConversacionIA> Conversaciones { get; }
    DbSet<MensajeIA> Mensajes { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
