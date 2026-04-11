using Microsoft.EntityFrameworkCore;
using PlanTA.Ventas.Application.Interfaces;
using PlanTA.Ventas.Domain.Entities;

namespace PlanTA.Ventas.Infrastructure.Data;

public class VentasDbContext : DbContext, IVentasDbContext
{
    public VentasDbContext(DbContextOptions<VentasDbContext> options) : base(options) { }

    public DbSet<Cliente> Clientes => Set<Cliente>();
    public DbSet<ContactoCliente> ContactosCliente => Set<ContactoCliente>();
    public DbSet<Pedido> Pedidos => Set<Pedido>();
    public DbSet<LineaPedido> LineasPedido => Set<LineaPedido>();
    public DbSet<Expedicion> Expediciones => Set<Expedicion>();
    public DbSet<LineaExpedicion> LineasExpedicion => Set<LineaExpedicion>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.HasDefaultSchema("ventas");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(VentasDbContext).Assembly);
    }
}
