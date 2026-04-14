using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PlanTA.Importador.Domain.Entities;

namespace PlanTA.Importador.Infrastructure.Data.Configurations;

public class ImportJobConfiguration : IEntityTypeConfiguration<ImportJob>
{
    public void Configure(EntityTypeBuilder<ImportJob> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasConversion(id => id.Value, v => new ImportJobId(v));

        builder.Property(x => x.NombreArchivo).HasMaxLength(300).IsRequired();
        builder.Property(x => x.ResumenErrores).HasMaxLength(4000);

        builder.HasIndex(x => new { x.EmpresaId, x.Estado });
    }
}
