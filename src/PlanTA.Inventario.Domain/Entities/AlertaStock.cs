using PlanTA.SharedKernel;

namespace PlanTA.Inventario.Domain.Entities;

public class AlertaStock : BaseEntity<AlertaStockId>
{
    private AlertaStock() { }

    public ProductoId ProductoId { get; private set; } = default!;
    public AlmacenId? AlmacenId { get; private set; }
    public decimal StockMinimo { get; private set; }
    public decimal StockMaximo { get; private set; }
    public decimal PuntoReorden { get; private set; }
    public decimal CantidadReorden { get; private set; }
    public bool AutoReorden { get; private set; }
    public bool Activa { get; private set; } = true;
    public Guid EmpresaId { get; private set; }

    public static AlertaStock Crear(
        ProductoId productoId, decimal stockMinimo, decimal stockMaximo,
        Guid empresaId, AlmacenId? almacenId = null,
        decimal puntoReorden = 0, decimal cantidadReorden = 0, bool autoReorden = false)
    {
        return new AlertaStock
        {
            Id = new AlertaStockId(Guid.NewGuid()),
            ProductoId = productoId,
            AlmacenId = almacenId,
            StockMinimo = stockMinimo,
            StockMaximo = stockMaximo,
            PuntoReorden = puntoReorden > 0 ? puntoReorden : stockMinimo,
            CantidadReorden = cantidadReorden,
            AutoReorden = autoReorden,
            EmpresaId = empresaId
        };
    }

    public void Actualizar(decimal stockMinimo, decimal stockMaximo, decimal puntoReorden, decimal cantidadReorden, bool autoReorden)
    {
        StockMinimo = stockMinimo;
        StockMaximo = stockMaximo;
        PuntoReorden = puntoReorden;
        CantidadReorden = cantidadReorden;
        AutoReorden = autoReorden;
        MarkUpdated();
    }

    public void Desactivar()
    {
        Activa = false;
        MarkUpdated();
    }
}
