using PlanTA.SharedKernel;

namespace PlanTA.Activos.Domain.Entities;

public class DocumentoActivo : BaseEntity<DocumentoActivoId>
{
    private DocumentoActivo() { }

    public ActivoId ActivoId { get; private set; } = default!;
    public string Nombre { get; private set; } = string.Empty;
    public string Tipo { get; private set; } = string.Empty;
    public string Url { get; private set; } = string.Empty;
    public long TamanoBytes { get; private set; }

    public static DocumentoActivo Crear(ActivoId activoId, string nombre, string tipo, string url, long tamanoBytes)
        => new()
        {
            Id = new DocumentoActivoId(Guid.NewGuid()),
            ActivoId = activoId,
            Nombre = nombre,
            Tipo = tipo,
            Url = url,
            TamanoBytes = tamanoBytes
        };
}
