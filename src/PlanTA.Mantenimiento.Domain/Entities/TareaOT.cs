using PlanTA.SharedKernel;

namespace PlanTA.Mantenimiento.Domain.Entities;

public class TareaOT : BaseEntity<TareaOTId>
{
    private TareaOT() { }

    public OrdenTrabajoId OrdenTrabajoId { get; private set; } = default!;
    public int Orden { get; private set; }
    public string Descripcion { get; private set; } = string.Empty;
    public bool Completada { get; private set; }
    public DateTimeOffset? CompletadaEn { get; private set; }
    public string? Notas { get; private set; }

    public static TareaOT Crear(OrdenTrabajoId otId, int orden, string descripcion)
        => new()
        {
            Id = new TareaOTId(Guid.NewGuid()),
            OrdenTrabajoId = otId,
            Orden = orden,
            Descripcion = descripcion
        };

    public void MarcarCompletada(string? notas = null)
    {
        Completada = true;
        CompletadaEn = DateTimeOffset.UtcNow;
        Notas = notas;
        MarkUpdated();
    }
}
