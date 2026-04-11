using PlanTA.Inventario.Domain.ValueObjects;
using PlanTA.SharedKernel;

namespace PlanTA.Inventario.Domain.Entities;

public class Ubicacion : BaseEntity<UbicacionId>
{
    private Ubicacion() { }

    public AlmacenId AlmacenId { get; private set; } = default!;
    public CodigoUbicacion Codigo { get; private set; } = default!;
    public string? Nombre { get; private set; }
    public int CapacidadMaxima { get; private set; }
    public bool Activa { get; private set; } = true;
    public Guid EmpresaId { get; private set; }

    internal static Ubicacion Crear(AlmacenId almacenId, string pasillo, string estante, string nivel, Guid empresaId, string? nombre = null, int capacidadMaxima = 0)
    {
        return new Ubicacion
        {
            Id = new UbicacionId(Guid.NewGuid()),
            AlmacenId = almacenId,
            Codigo = CodigoUbicacion.Create(pasillo, estante, nivel),
            Nombre = nombre,
            CapacidadMaxima = capacidadMaxima,
            EmpresaId = empresaId
        };
    }

    public void Desactivar()
    {
        Activa = false;
        MarkUpdated();
    }
}
