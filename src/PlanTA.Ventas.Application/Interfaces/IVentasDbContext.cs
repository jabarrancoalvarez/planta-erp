using Microsoft.EntityFrameworkCore;
using PlanTA.Ventas.Domain.Entities;

namespace PlanTA.Ventas.Application.Interfaces;

public interface IVentasDbContext
{
    DbSet<Cliente> Clientes { get; }
    DbSet<ContactoCliente> ContactosCliente { get; }
    DbSet<Pedido> Pedidos { get; }
    DbSet<LineaPedido> LineasPedido { get; }
    DbSet<Expedicion> Expediciones { get; }
    DbSet<LineaExpedicion> LineasExpedicion { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
