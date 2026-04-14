using PlanTA.SharedKernel;

namespace PlanTA.Incidencias.Domain.Entities;

public class ComentarioIncidencia : BaseEntity<ComentarioIncidenciaId>
{
    private ComentarioIncidencia() { }

    public IncidenciaId IncidenciaId { get; private set; } = default!;
    public Guid UserId { get; private set; }
    public string Texto { get; private set; } = string.Empty;

    public static ComentarioIncidencia Crear(IncidenciaId incId, Guid userId, string texto)
        => new()
        {
            Id = new ComentarioIncidenciaId(Guid.NewGuid()),
            IncidenciaId = incId,
            UserId = userId,
            Texto = texto
        };
}
