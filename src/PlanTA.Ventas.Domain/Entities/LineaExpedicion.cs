using PlanTA.SharedKernel;

namespace PlanTA.Ventas.Domain.Entities;

public class LineaExpedicion : BaseEntity<LineaExpedicionId>
{
    private LineaExpedicion() { }

    public ExpedicionId ExpedicionId { get; private set; } = default!;
    public LineaPedidoId LineaPedidoId { get; private set; } = default!;
    public Guid ProductoId { get; private set; }
    public decimal Cantidad { get; private set; }
    public Guid? LoteOrigenId { get; private set; }

    internal static LineaExpedicion Crear(
        ExpedicionId expedicionId,
        LineaPedidoId lineaPedidoId,
        Guid productoId,
        decimal cantidad,
        Guid? loteOrigenId = null)
    {
        return new LineaExpedicion
        {
            Id = new LineaExpedicionId(Guid.NewGuid()),
            ExpedicionId = expedicionId,
            LineaPedidoId = lineaPedidoId,
            ProductoId = productoId,
            Cantidad = cantidad,
            LoteOrigenId = loteOrigenId
        };
    }
}
