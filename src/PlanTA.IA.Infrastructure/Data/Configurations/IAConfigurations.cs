using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PlanTA.IA.Domain.Entities;

namespace PlanTA.IA.Infrastructure.Data.Configurations;

public class ConversacionIAConfiguration : IEntityTypeConfiguration<ConversacionIA>
{
    public void Configure(EntityTypeBuilder<ConversacionIA> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasConversion(id => id.Value, v => new ConversacionIAId(v));
        builder.Property(x => x.Titulo).HasMaxLength(300).IsRequired();

        builder.HasMany(x => x.Mensajes)
            .WithOne()
            .HasForeignKey(m => m.ConversacionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Metadata.FindNavigation(nameof(ConversacionIA.Mensajes))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);

        builder.HasIndex(x => new { x.EmpresaId, x.UsuarioId });
        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}

public class MensajeIAConfiguration : IEntityTypeConfiguration<MensajeIA>
{
    public void Configure(EntityTypeBuilder<MensajeIA> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasConversion(id => id.Value, v => new MensajeIAId(v));
        builder.Property(x => x.ConversacionId).HasConversion(id => id.Value, v => new ConversacionIAId(v));

        builder.Property(x => x.Contenido).IsRequired();
        builder.Property(x => x.Modelo).HasMaxLength(100);
    }
}
