using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PlanTA.Ventas.Domain.Entities;

namespace PlanTA.Ventas.Infrastructure.Data.Configurations;

public class ContactoClienteConfiguration : IEntityTypeConfiguration<ContactoCliente>
{
    public void Configure(EntityTypeBuilder<ContactoCliente> builder)
    {
        builder.ToTable("contactos_cliente");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasConversion(id => id.Value, v => new ContactoClienteId(v));

        builder.Property(x => x.ClienteId)
            .HasConversion(id => id.Value, v => new ClienteId(v));

        builder.Property(x => x.Nombre).HasMaxLength(200).IsRequired();
        builder.Property(x => x.Cargo).HasMaxLength(100);
        builder.Property(x => x.Email).HasMaxLength(200).IsRequired();
        builder.Property(x => x.Telefono).HasMaxLength(50);
    }
}
