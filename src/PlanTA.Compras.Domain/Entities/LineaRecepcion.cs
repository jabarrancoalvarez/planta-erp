using PlanTA.SharedKernel;

namespace PlanTA.Compras.Domain.Entities;

public class LineaRecepcion : BaseEntity<LineaRecepcionId>
{
    private LineaRecepcion() { }

    public RecepcionId RecepcionId { get; private set; } = default!;
    public LineaOrdenCompraId LineaOrdenCompraId { get; private set; } = default!;
    public Guid ProductoId { get; private set; }
    public decimal CantidadRecibida { get; private set; }
    public Guid? LoteId { get; private set; }
    public Guid? UbicacionDestinoId { get; private set; }

    internal static LineaRecepcion Crear(
        RecepcionId recepcionId,
        LineaOrdenCompraId lineaOrdenCompraId,
        Guid productoId,
        decimal cantidadRecibida,
        Guid? loteId = null,
        Guid? ubicacionDestinoId = null)
    {
        return new LineaRecepcion
        {
            Id = new LineaRecepcionId(Guid.NewGuid()),
            RecepcionId = recepcionId,
            LineaOrdenCompraId = lineaOrdenCompraId,
            ProductoId = productoId,
            CantidadRecibida = cantidadRecibida,
            LoteId = loteId,
            UbicacionDestinoId = ubicacionDestinoId
        };
    }
}
