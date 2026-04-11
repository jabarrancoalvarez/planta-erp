using PlanTA.SharedKernel;

namespace PlanTA.Ventas.Domain.Entities;

public class LineaPedido : BaseEntity<LineaPedidoId>
{
    private LineaPedido() { }

    public PedidoId PedidoId { get; private set; } = default!;
    public Guid ProductoId { get; private set; }
    public string Descripcion { get; private set; } = string.Empty;
    public decimal Cantidad { get; private set; }
    public decimal PrecioUnitario { get; private set; }
    public decimal Descuento { get; private set; }
    public decimal CantidadEnviada { get; private set; }

    public decimal Total => Cantidad * PrecioUnitario * (1 - Descuento / 100);
    public decimal CantidadPendiente => Cantidad - CantidadEnviada;

    internal static LineaPedido Crear(
        PedidoId pedidoId,
        Guid productoId,
        string descripcion,
        decimal cantidad,
        decimal precioUnitario,
        decimal descuento = 0)
    {
        return new LineaPedido
        {
            Id = new LineaPedidoId(Guid.NewGuid()),
            PedidoId = pedidoId,
            ProductoId = productoId,
            Descripcion = descripcion,
            Cantidad = cantidad,
            PrecioUnitario = precioUnitario,
            Descuento = descuento,
            CantidadEnviada = 0
        };
    }

    public void RegistrarEnvio(decimal cantidad)
    {
        CantidadEnviada += cantidad;
        MarkUpdated();
    }
}
