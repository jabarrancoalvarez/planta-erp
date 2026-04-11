using Microsoft.EntityFrameworkCore;
using PlanTA.Produccion.Application.Interfaces;
using PlanTA.Produccion.Domain.Entities;

namespace PlanTA.Produccion.Infrastructure.Data;

public class ProduccionDbContext : DbContext, IProduccionDbContext
{
    public ProduccionDbContext(DbContextOptions<ProduccionDbContext> options) : base(options) { }

    public DbSet<ListaMateriales> ListasMateriales => Set<ListaMateriales>();
    public DbSet<LineaBOM> LineasBOM => Set<LineaBOM>();
    public DbSet<RutaProduccion> RutasProduccion => Set<RutaProduccion>();
    public DbSet<OperacionRuta> OperacionesRuta => Set<OperacionRuta>();
    public DbSet<OrdenFabricacion> OrdenesFabricacion => Set<OrdenFabricacion>();
    public DbSet<LineaConsumoOF> LineasConsumoOF => Set<LineaConsumoOF>();
    public DbSet<ParteProduccion> PartesProduccion => Set<ParteProduccion>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.HasDefaultSchema("produccion");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ProduccionDbContext).Assembly);
    }
}
