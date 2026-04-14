using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PlanTA.Mantenimiento.Domain.Entities;

namespace PlanTA.Mantenimiento.Infrastructure.Data.Configurations;

public class OrdenTrabajoConfiguration : IEntityTypeConfiguration<OrdenTrabajo>
{
    public void Configure(EntityTypeBuilder<OrdenTrabajo> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasConversion(id => id.Value, v => new OrdenTrabajoId(v));

        builder.Property(x => x.Codigo).HasMaxLength(50).IsRequired();
        builder.HasIndex(x => new { x.EmpresaId, x.Codigo }).IsUnique();

        builder.Property(x => x.Titulo).HasMaxLength(200).IsRequired();
        builder.Property(x => x.Descripcion).HasMaxLength(2000);
        builder.Property(x => x.NotasCierre).HasMaxLength(2000);
        builder.Property(x => x.HorasEstimadas).HasPrecision(18, 4);
        builder.Property(x => x.HorasReales).HasPrecision(18, 4);
        builder.Property(x => x.CosteManoObra).HasPrecision(18, 4);
        builder.Property(x => x.CosteRepuestos).HasPrecision(18, 4);

        builder.HasIndex(x => x.EmpresaId);
        builder.HasIndex(x => x.ActivoId);
        builder.HasIndex(x => x.Estado);
        builder.HasQueryFilter(x => !x.IsDeleted);

        builder.HasMany(x => x.Tareas)
            .WithOne()
            .HasForeignKey(t => t.OrdenTrabajoId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.Repuestos)
            .WithOne()
            .HasForeignKey(r => r.OrdenTrabajoId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class TareaOTConfiguration : IEntityTypeConfiguration<TareaOT>
{
    public void Configure(EntityTypeBuilder<TareaOT> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasConversion(id => id.Value, v => new TareaOTId(v));
        builder.Property(x => x.OrdenTrabajoId).HasConversion(id => id.Value, v => new OrdenTrabajoId(v));
        builder.Property(x => x.Descripcion).HasMaxLength(500).IsRequired();
        builder.Property(x => x.Notas).HasMaxLength(1000);
    }
}

public class RepuestoOTConfiguration : IEntityTypeConfiguration<RepuestoOT>
{
    public void Configure(EntityTypeBuilder<RepuestoOT> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasConversion(id => id.Value, v => new RepuestoOTId(v));
        builder.Property(x => x.OrdenTrabajoId).HasConversion(id => id.Value, v => new OrdenTrabajoId(v));
        builder.Property(x => x.Descripcion).HasMaxLength(300).IsRequired();
        builder.Property(x => x.Cantidad).HasPrecision(18, 4);
        builder.Property(x => x.CosteUnitario).HasPrecision(18, 4);
        builder.Ignore(x => x.CosteTotal);
    }
}

public class PlanMantenimientoConfiguration : IEntityTypeConfiguration<PlanMantenimiento>
{
    public void Configure(EntityTypeBuilder<PlanMantenimiento> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasConversion(id => id.Value, v => new PlanMantenimientoId(v));
        builder.Property(x => x.Codigo).HasMaxLength(50).IsRequired();
        builder.HasIndex(x => new { x.EmpresaId, x.Codigo }).IsUnique();
        builder.Property(x => x.Nombre).HasMaxLength(200).IsRequired();
        builder.Property(x => x.Descripcion).HasMaxLength(1000);
        builder.Property(x => x.HorasEstimadas).HasPrecision(18, 4);
        builder.Property(x => x.UmbralHorasUso).HasPrecision(18, 4);
        builder.HasIndex(x => x.EmpresaId);
        builder.HasIndex(x => x.ActivoId);
        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
