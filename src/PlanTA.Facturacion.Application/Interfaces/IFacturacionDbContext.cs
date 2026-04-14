using Microsoft.EntityFrameworkCore;
using PlanTA.Facturacion.Domain.Entities;

namespace PlanTA.Facturacion.Application.Interfaces;

public interface IFacturacionDbContext
{
    DbSet<Factura> Facturas { get; }
    DbSet<LineaFactura> Lineas { get; }
    DbSet<SerieFactura> Series { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
