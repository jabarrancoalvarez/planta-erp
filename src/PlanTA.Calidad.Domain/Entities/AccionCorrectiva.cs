using PlanTA.SharedKernel;

namespace PlanTA.Calidad.Domain.Entities;

public class AccionCorrectiva : BaseEntity<AccionCorrectivaId>
{
    private AccionCorrectiva() { }

    public NoConformidadId NoConformidadId { get; private set; } = default!;
    public string Descripcion { get; private set; } = string.Empty;
    public string? ResponsableUserId { get; private set; }
    public DateTimeOffset? FechaLimite { get; private set; }
    public DateTimeOffset? FechaCompletada { get; private set; }
    public bool Completada { get; private set; }

    public static AccionCorrectiva Crear(
        NoConformidadId noConformidadId,
        string descripcion,
        string? responsableUserId = null,
        DateTimeOffset? fechaLimite = null)
    {
        return new AccionCorrectiva
        {
            Id = new AccionCorrectivaId(Guid.NewGuid()),
            NoConformidadId = noConformidadId,
            Descripcion = descripcion,
            ResponsableUserId = responsableUserId,
            FechaLimite = fechaLimite
        };
    }

    public void Completar()
    {
        Completada = true;
        FechaCompletada = DateTimeOffset.UtcNow;
        MarkUpdated();
    }
}
