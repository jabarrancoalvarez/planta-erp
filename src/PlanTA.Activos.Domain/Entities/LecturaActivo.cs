using PlanTA.SharedKernel;

namespace PlanTA.Activos.Domain.Entities;

public class LecturaActivo : BaseEntity<LecturaActivoId>
{
    private LecturaActivo() { }

    public ActivoId ActivoId { get; private set; } = default!;
    public string Tipo { get; private set; } = string.Empty;
    public decimal Valor { get; private set; }
    public string? Unidad { get; private set; }
    public DateTimeOffset FechaLectura { get; private set; }
    public string? Notas { get; private set; }

    public static LecturaActivo Crear(ActivoId activoId, string tipo, decimal valor, string? unidad, string? notas = null)
        => new()
        {
            Id = new LecturaActivoId(Guid.NewGuid()),
            ActivoId = activoId,
            Tipo = tipo,
            Valor = valor,
            Unidad = unidad,
            Notas = notas,
            FechaLectura = DateTimeOffset.UtcNow
        };
}
