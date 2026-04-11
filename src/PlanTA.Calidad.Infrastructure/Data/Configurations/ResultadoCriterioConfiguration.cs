using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PlanTA.Calidad.Domain.Entities;

namespace PlanTA.Calidad.Infrastructure.Data.Configurations;

public class ResultadoCriterioConfiguration : IEntityTypeConfiguration<ResultadoCriterio>
{
    public void Configure(EntityTypeBuilder<ResultadoCriterio> builder)
    {
        builder.ToTable("resultados_criterio");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasConversion(id => id.Value, v => new ResultadoCriterioId(v));
        builder.Property(x => x.InspeccionId).HasConversion(id => id.Value, v => new InspeccionId(v));
        builder.Property(x => x.CriterioInspeccionId).HasConversion(id => id.Value, v => new CriterioInspeccionId(v));

        builder.Property(x => x.ValorMedido).HasMaxLength(500);
        builder.Property(x => x.NombreCriterio).HasMaxLength(300).IsRequired();
        builder.Property(x => x.Observaciones).HasMaxLength(2000);
    }
}
