using Microsoft.EntityFrameworkCore;
using PlanTA.Facturacion.Application.Interfaces;
using PlanTA.Facturacion.Domain.Entities;

namespace PlanTA.Facturacion.Infrastructure.Data;

public class FacturacionDbContext : DbContext, IFacturacionDbContext
{
    public FacturacionDbContext(DbContextOptions<FacturacionDbContext> options) : base(options) { }

    public DbSet<Factura> Facturas => Set<Factura>();
    public DbSet<LineaFactura> Lineas => Set<LineaFactura>();
    public DbSet<SerieFactura> Series => Set<SerieFactura>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.HasDefaultSchema("facturacion");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(FacturacionDbContext).Assembly);
    }
}
