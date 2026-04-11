using Microsoft.EntityFrameworkCore;
using PlanTA.Inventario.Domain.Entities;

namespace PlanTA.Inventario.Application.Interfaces;

public interface IInventarioDbContext
{
    DbSet<Producto> Productos { get; }
    DbSet<CategoriaProducto> Categorias { get; }
    DbSet<Almacen> Almacenes { get; }
    DbSet<Ubicacion> Ubicaciones { get; }
    DbSet<Lote> Lotes { get; }
    DbSet<MovimientoStock> Movimientos { get; }
    DbSet<AlertaStock> Alertas { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
