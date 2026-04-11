using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PlanTA.Compras.Domain.Entities;

namespace PlanTA.Compras.Infrastructure.Data.Configurations;

public class OrdenCompraConfiguration : IEntityTypeConfiguration<OrdenCompra>
{
    public void Configure(EntityTypeBuilder<OrdenCompra> builder)
    {
        builder.ToTable("ordenes_compra");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasConversion(id => id.Value, v => new OrdenCompraId(v));

        builder.Property(x => x.Codigo).HasMaxLength(50).IsRequired();
        builder.HasIndex(x => new { x.Codigo, x.EmpresaId }).IsUnique();

        builder.Property(x => x.ProveedorId)
            .HasConversion(id => id.Value, v => new ProveedorId(v));

        builder.Property(x => x.Observaciones).HasMaxLength(2000);

        builder.HasMany(x => x.Lineas)
            .WithOne()
            .HasForeignKey(l => l.OrdenCompraId);

        builder.HasIndex(x => x.EmpresaId);
        builder.HasIndex(x => x.ProveedorId);
        builder.HasIndex(x => x.EstadoOC);
        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
