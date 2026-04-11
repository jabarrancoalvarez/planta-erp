using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PlanTA.Calidad.Domain.Entities;

namespace PlanTA.Calidad.Infrastructure.Data.Configurations;

public class AccionCorrectivaConfiguration : IEntityTypeConfiguration<AccionCorrectiva>
{
    public void Configure(EntityTypeBuilder<AccionCorrectiva> builder)
    {
        builder.ToTable("acciones_correctivas");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasConversion(id => id.Value, v => new AccionCorrectivaId(v));
        builder.Property(x => x.NoConformidadId).HasConversion(id => id.Value, v => new NoConformidadId(v));

        builder.Property(x => x.Descripcion).HasMaxLength(2000).IsRequired();
        builder.Property(x => x.ResponsableUserId).HasMaxLength(200);
    }
}
