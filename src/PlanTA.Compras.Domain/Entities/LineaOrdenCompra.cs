using PlanTA.SharedKernel;

namespace PlanTA.Compras.Domain.Entities;

public class LineaOrdenCompra : BaseEntity<LineaOrdenCompraId>
{
    private LineaOrdenCompra() { }

    public OrdenCompraId OrdenCompraId { get; private set; } = default!;
    public Guid ProductoId { get; private set; }
    public string Descripcion { get; private set; } = string.Empty;
    public decimal Cantidad { get; private set; }
    public decimal PrecioUnitario { get; private set; }
    public decimal CantidadRecibida { get; private set; }

    public decimal Total => Cantidad * PrecioUnitario;
    public decimal CantidadPendiente => Cantidad - CantidadRecibida;

    internal static LineaOrdenCompra Crear(
        OrdenCompraId ordenCompraId,
        Guid productoId,
        string descripcion,
        decimal cantidad,
        decimal precioUnitario)
    {
        return new LineaOrdenCompra
        {
            Id = new LineaOrdenCompraId(Guid.NewGuid()),
            OrdenCompraId = ordenCompraId,
            ProductoId = productoId,
            Descripcion = descripcion,
            Cantidad = cantidad,
            PrecioUnitario = precioUnitario,
            CantidadRecibida = 0
        };
    }

    public void RegistrarRecepcion(decimal cantidad)
    {
        CantidadRecibida += cantidad;
        MarkUpdated();
    }
}
