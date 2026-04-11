using PlanTA.SharedKernel;

namespace PlanTA.Produccion.Domain.Entities;

public class LineaConsumoOF : BaseEntity<LineaConsumoOFId>
{
    private LineaConsumoOF() { }

    public OrdenFabricacionId OrdenFabricacionId { get; private set; } = default!;
    public Guid ProductoId { get; private set; }
    public Guid? LoteId { get; private set; }
    public decimal Cantidad { get; private set; }
    public DateTimeOffset FechaConsumo { get; private set; }

    internal static LineaConsumoOF Crear(
        OrdenFabricacionId ordenFabricacionId,
        Guid productoId,
        decimal cantidad,
        Guid? loteId = null)
    {
        return new LineaConsumoOF
        {
            Id = new LineaConsumoOFId(Guid.NewGuid()),
            OrdenFabricacionId = ordenFabricacionId,
            ProductoId = productoId,
            LoteId = loteId,
            Cantidad = cantidad,
            FechaConsumo = DateTimeOffset.UtcNow
        };
    }
}
