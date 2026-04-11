using Microsoft.EntityFrameworkCore;
using PlanTA.Produccion.Domain.Entities;

namespace PlanTA.Produccion.Application.Interfaces;

public interface IProduccionDbContext
{
    DbSet<ListaMateriales> ListasMateriales { get; }
    DbSet<LineaBOM> LineasBOM { get; }
    DbSet<RutaProduccion> RutasProduccion { get; }
    DbSet<OperacionRuta> OperacionesRuta { get; }
    DbSet<OrdenFabricacion> OrdenesFabricacion { get; }
    DbSet<LineaConsumoOF> LineasConsumoOF { get; }
    DbSet<ParteProduccion> PartesProduccion { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
