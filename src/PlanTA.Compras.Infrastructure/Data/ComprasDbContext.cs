using Microsoft.EntityFrameworkCore;
using PlanTA.Compras.Application.Interfaces;
using PlanTA.Compras.Domain.Entities;

namespace PlanTA.Compras.Infrastructure.Data;

public class ComprasDbContext : DbContext, IComprasDbContext
{
    public ComprasDbContext(DbContextOptions<ComprasDbContext> options) : base(options) { }

    public DbSet<Proveedor> Proveedores => Set<Proveedor>();
    public DbSet<ContactoProveedor> ContactosProveedor => Set<ContactoProveedor>();
    public DbSet<OrdenCompra> OrdenesCompra => Set<OrdenCompra>();
    public DbSet<LineaOrdenCompra> LineasOrdenCompra => Set<LineaOrdenCompra>();
    public DbSet<Recepcion> Recepciones => Set<Recepcion>();
    public DbSet<LineaRecepcion> LineasRecepcion => Set<LineaRecepcion>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.HasDefaultSchema("compras");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ComprasDbContext).Assembly);
    }
}
