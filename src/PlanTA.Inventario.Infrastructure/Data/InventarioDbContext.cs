using Microsoft.EntityFrameworkCore;
using PlanTA.Inventario.Application.Interfaces;
using PlanTA.Inventario.Domain.Entities;

namespace PlanTA.Inventario.Infrastructure.Data;

public class InventarioDbContext : DbContext, IInventarioDbContext
{
    public InventarioDbContext(DbContextOptions<InventarioDbContext> options) : base(options) { }

    public DbSet<Producto> Productos => Set<Producto>();
    public DbSet<CategoriaProducto> Categorias => Set<CategoriaProducto>();
    public DbSet<Almacen> Almacenes => Set<Almacen>();
    public DbSet<Ubicacion> Ubicaciones => Set<Ubicacion>();
    public DbSet<Lote> Lotes => Set<Lote>();
    public DbSet<MovimientoStock> Movimientos => Set<MovimientoStock>();
    public DbSet<AlertaStock> Alertas => Set<AlertaStock>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.HasDefaultSchema("inventario");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(InventarioDbContext).Assembly);
    }
}
