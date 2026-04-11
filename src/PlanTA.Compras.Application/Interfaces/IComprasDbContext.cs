using Microsoft.EntityFrameworkCore;
using PlanTA.Compras.Domain.Entities;

namespace PlanTA.Compras.Application.Interfaces;

public interface IComprasDbContext
{
    DbSet<Proveedor> Proveedores { get; }
    DbSet<ContactoProveedor> ContactosProveedor { get; }
    DbSet<OrdenCompra> OrdenesCompra { get; }
    DbSet<LineaOrdenCompra> LineasOrdenCompra { get; }
    DbSet<Recepcion> Recepciones { get; }
    DbSet<LineaRecepcion> LineasRecepcion { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
