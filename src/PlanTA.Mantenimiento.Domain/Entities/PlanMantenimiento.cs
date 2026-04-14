using PlanTA.Mantenimiento.Domain.Enums;
using PlanTA.SharedKernel;

namespace PlanTA.Mantenimiento.Domain.Entities;

public class PlanMantenimiento : SoftDeletableEntity<PlanMantenimientoId>
{
    private PlanMantenimiento() { }

    public string Codigo { get; private set; } = string.Empty;
    public string Nombre { get; private set; } = string.Empty;
    public string? Descripcion { get; private set; }
    public Guid ActivoId { get; private set; }
    public TipoMantenimiento Tipo { get; private set; } = TipoMantenimiento.Preventivo;
    public FrecuenciaPlan Frecuencia { get; private set; }
    public int Intervalo { get; private set; }
    public decimal? UmbralHorasUso { get; private set; }
    public decimal HorasEstimadas { get; private set; }
    public DateTimeOffset? ProximaEjecucion { get; private set; }
    public DateTimeOffset? UltimaEjecucion { get; private set; }
    public bool Activo { get; private set; } = true;
    public Guid EmpresaId { get; private set; }

    public static PlanMantenimiento Crear(
        string codigo, string nombre, Guid activoId, FrecuenciaPlan frecuencia,
        int intervalo, Guid empresaId, string? descripcion = null,
        decimal horasEstimadas = 0, decimal? umbralHorasUso = null,
        DateTimeOffset? proximaEjecucion = null)
        => new()
        {
            Id = new PlanMantenimientoId(Guid.NewGuid()),
            Codigo = codigo.Trim().ToUpperInvariant(),
            Nombre = nombre.Trim(),
            Descripcion = descripcion,
            ActivoId = activoId,
            Frecuencia = frecuencia,
            Intervalo = intervalo,
            HorasEstimadas = horasEstimadas,
            UmbralHorasUso = umbralHorasUso,
            ProximaEjecucion = proximaEjecucion ?? DateTimeOffset.UtcNow,
            EmpresaId = empresaId
        };

    public void RegistrarEjecucion(DateTimeOffset fecha, DateTimeOffset? siguiente)
    {
        UltimaEjecucion = fecha;
        ProximaEjecucion = siguiente;
        MarkUpdated();
    }

    public void Desactivar() { Activo = false; MarkUpdated(); }
    public void Activar() { Activo = true; MarkUpdated(); }
}
