using PlanTA.SharedKernel;

namespace PlanTA.Produccion.Domain.Entities;

public class LineaBOM : BaseEntity<LineaBOMId>
{
    private LineaBOM() { }

    public ListaMaterialesId ListaMaterialesId { get; private set; } = default!;
    public Guid ComponenteProductoId { get; private set; }
    public decimal Cantidad { get; private set; }
    public string UnidadMedida { get; private set; } = string.Empty;
    public decimal Merma { get; private set; }
    public int Orden { get; private set; }

    internal static LineaBOM Crear(
        ListaMaterialesId listaMaterialesId,
        Guid componenteProductoId,
        decimal cantidad,
        string unidadMedida,
        decimal merma,
        int orden)
    {
        return new LineaBOM
        {
            Id = new LineaBOMId(Guid.NewGuid()),
            ListaMaterialesId = listaMaterialesId,
            ComponenteProductoId = componenteProductoId,
            Cantidad = cantidad,
            UnidadMedida = unidadMedida,
            Merma = merma,
            Orden = orden
        };
    }
}
