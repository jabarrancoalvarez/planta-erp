using PlanTA.SharedKernel;

namespace PlanTA.Incidencias.Domain.Entities;

public class FotoIncidencia : BaseEntity<FotoIncidenciaId>
{
    private FotoIncidencia() { }

    public IncidenciaId IncidenciaId { get; private set; } = default!;
    public string Url { get; private set; } = string.Empty;
    public string? Descripcion { get; private set; }

    public static FotoIncidencia Crear(IncidenciaId incId, string url, string? descripcion = null)
        => new()
        {
            Id = new FotoIncidenciaId(Guid.NewGuid()),
            IncidenciaId = incId,
            Url = url,
            Descripcion = descripcion
        };
}
