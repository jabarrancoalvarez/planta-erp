using PlanTA.SharedKernel;

namespace PlanTA.Facturacion.Domain.Entities;

public class LineaFactura : BaseEntity<LineaFacturaId>
{
    private LineaFactura() { }

    public FacturaId FacturaId { get; private set; } = default!;
    public int NumeroLinea { get; private set; }
    public string Descripcion { get; private set; } = string.Empty;
    public decimal Cantidad { get; private set; }
    public decimal PrecioUnitario { get; private set; }
    public decimal DescuentoPct { get; private set; }
    public decimal IvaPct { get; private set; }
    public Guid? ProductoId { get; private set; }

    public decimal BaseImponible => Math.Round(Cantidad * PrecioUnitario * (1 - DescuentoPct / 100m), 2);
    public decimal Iva => Math.Round(BaseImponible * IvaPct / 100m, 2);
    public decimal Total => BaseImponible + Iva;

    public static LineaFactura Crear(
        FacturaId facturaId, int numeroLinea, string descripcion,
        decimal cantidad, decimal precioUnitario, decimal ivaPct,
        decimal descuentoPct = 0, Guid? productoId = null)
        => new()
        {
            Id = new LineaFacturaId(Guid.NewGuid()),
            FacturaId = facturaId,
            NumeroLinea = numeroLinea,
            Descripcion = descripcion,
            Cantidad = cantidad,
            PrecioUnitario = precioUnitario,
            IvaPct = ivaPct,
            DescuentoPct = descuentoPct,
            ProductoId = productoId
        };
}
