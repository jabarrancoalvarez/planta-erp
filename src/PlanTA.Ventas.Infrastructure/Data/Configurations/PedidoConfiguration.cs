using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PlanTA.Ventas.Domain.Entities;

namespace PlanTA.Ventas.Infrastructure.Data.Configurations;

public class PedidoConfiguration : IEntityTypeConfiguration<Pedido>
{
    public void Configure(EntityTypeBuilder<Pedido> builder)
    {
        builder.ToTable("pedidos");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasConversion(id => id.Value, v => new PedidoId(v));

        builder.Property(x => x.Codigo).HasMaxLength(50).IsRequired();
        builder.HasIndex(x => new { x.Codigo, x.EmpresaId }).IsUnique();

        builder.Property(x => x.ClienteId)
            .HasConversion(id => id.Value, v => new ClienteId(v));

        builder.Property(x => x.DireccionEntrega).HasMaxLength(500);
        builder.Property(x => x.Observaciones).HasMaxLength(2000);

        builder.HasMany(x => x.Lineas)
            .WithOne()
            .HasForeignKey(l => l.PedidoId);

        builder.HasIndex(x => x.EmpresaId);
        builder.HasIndex(x => x.ClienteId);
        builder.HasIndex(x => x.EstadoPedido);
        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
