using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PlanTA.Ventas.Domain.Entities;

namespace PlanTA.Ventas.Infrastructure.Data.Configurations;

public class ClienteConfiguration : IEntityTypeConfiguration<Cliente>
{
    public void Configure(EntityTypeBuilder<Cliente> builder)
    {
        builder.ToTable("clientes");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasConversion(id => id.Value, v => new ClienteId(v));

        builder.Property(x => x.RazonSocial).HasMaxLength(300).IsRequired();
        builder.Property(x => x.NIF).HasMaxLength(20).IsRequired();
        builder.Property(x => x.DireccionEnvio).HasMaxLength(500);
        builder.Property(x => x.DireccionFacturacion).HasMaxLength(500);
        builder.Property(x => x.Ciudad).HasMaxLength(100);
        builder.Property(x => x.CodigoPostal).HasMaxLength(10);
        builder.Property(x => x.Pais).HasMaxLength(100);
        builder.Property(x => x.Email).HasMaxLength(200).IsRequired();
        builder.Property(x => x.Telefono).HasMaxLength(50);
        builder.Property(x => x.Web).HasMaxLength(300);

        builder.HasMany(x => x.Contactos)
            .WithOne()
            .HasForeignKey(c => c.ClienteId);

        builder.HasIndex(x => new { x.NIF, x.EmpresaId }).IsUnique();
        builder.HasIndex(x => x.EmpresaId);
        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
