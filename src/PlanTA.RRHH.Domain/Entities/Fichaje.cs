using PlanTA.RRHH.Domain.Enums;
using PlanTA.SharedKernel;

namespace PlanTA.RRHH.Domain.Entities;

public class Fichaje : BaseEntity<FichajeId>
{
    private Fichaje() { }

    public EmpleadoId EmpleadoId { get; private set; } = default!;
    public TipoFichaje Tipo { get; private set; }
    public DateTimeOffset Momento { get; private set; }
    public Guid? ActivoId { get; private set; }
    public Guid? OrdenFabricacionId { get; private set; }
    public Guid? OrdenTrabajoId { get; private set; }
    public string? Notas { get; private set; }
    public Guid EmpresaId { get; private set; }

    public static Fichaje Crear(
        EmpleadoId empleadoId, TipoFichaje tipo, Guid empresaId,
        Guid? activoId = null, Guid? ordenFabricacionId = null, Guid? ordenTrabajoId = null, string? notas = null)
        => new()
        {
            Id = new FichajeId(Guid.NewGuid()),
            EmpleadoId = empleadoId,
            Tipo = tipo,
            Momento = DateTimeOffset.UtcNow,
            ActivoId = activoId,
            OrdenFabricacionId = ordenFabricacionId,
            OrdenTrabajoId = ordenTrabajoId,
            Notas = notas,
            EmpresaId = empresaId
        };
}
