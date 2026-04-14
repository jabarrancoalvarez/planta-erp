using PlanTA.SharedKernel;

namespace PlanTA.RRHH.Domain.Entities;

public class Turno : SoftDeletableEntity<TurnoId>
{
    private Turno() { }

    public string Nombre { get; private set; } = string.Empty;
    public TimeOnly HoraInicio { get; private set; }
    public TimeOnly HoraFin { get; private set; }
    public int MinutosDescanso { get; private set; }
    public string DiasSemana { get; private set; } = "LMXJV";
    public bool Activo { get; private set; } = true;
    public Guid EmpresaId { get; private set; }

    public static Turno Crear(string nombre, TimeOnly inicio, TimeOnly fin, Guid empresaId,
        int minutosDescanso = 0, string diasSemana = "LMXJV")
        => new()
        {
            Id = new TurnoId(Guid.NewGuid()),
            Nombre = nombre,
            HoraInicio = inicio,
            HoraFin = fin,
            MinutosDescanso = minutosDescanso,
            DiasSemana = diasSemana,
            EmpresaId = empresaId
        };
}
