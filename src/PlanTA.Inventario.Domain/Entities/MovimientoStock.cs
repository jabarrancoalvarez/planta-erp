using PlanTA.Inventario.Domain.Enums;
using PlanTA.Inventario.Domain.Events;
using PlanTA.SharedKernel;

namespace PlanTA.Inventario.Domain.Entities;

/// <summary>Registro inmutable de cada movimiento de stock.</summary>
public class MovimientoStock : AggregateRoot<MovimientoStockId>
{
    private MovimientoStock() { }

    public ProductoId ProductoId { get; private set; } = default!;
    public AlmacenId AlmacenId { get; private set; } = default!;
    public UbicacionId? UbicacionId { get; private set; }
    public LoteId? LoteId { get; private set; }
    public TipoMovimiento Tipo { get; private set; }
    public decimal Cantidad { get; private set; }
    public decimal CantidadAnterior { get; private set; }
    public decimal CantidadPosterior { get; private set; }
    public string? Referencia { get; private set; }
    public string? Notas { get; private set; }
    public Guid EmpresaId { get; private set; }

    public static MovimientoStock Registrar(
        ProductoId productoId, AlmacenId almacenId, TipoMovimiento tipo,
        decimal cantidad, decimal cantidadAnterior, Guid empresaId,
        UbicacionId? ubicacionId = null, LoteId? loteId = null,
        string? referencia = null, string? notas = null)
    {
        var cantidadPosterior = tipo switch
        {
            TipoMovimiento.Entrada or TipoMovimiento.Devolucion => cantidadAnterior + cantidad,
            TipoMovimiento.Salida or TipoMovimiento.Merma or TipoMovimiento.Reserva => cantidadAnterior - cantidad,
            TipoMovimiento.Ajuste => cantidad, // cantidad es el nuevo total
            TipoMovimiento.TransferenciaInterna => cantidadAnterior, // no cambia en origen (se crea otro en destino)
            _ => cantidadAnterior
        };

        var movimiento = new MovimientoStock
        {
            Id = new MovimientoStockId(Guid.NewGuid()),
            ProductoId = productoId,
            AlmacenId = almacenId,
            UbicacionId = ubicacionId,
            LoteId = loteId,
            Tipo = tipo,
            Cantidad = cantidad,
            CantidadAnterior = cantidadAnterior,
            CantidadPosterior = cantidadPosterior,
            Referencia = referencia,
            Notas = notas,
            EmpresaId = empresaId
        };

        movimiento.AddDomainEvent(new StockActualizadoEvent(
            productoId, almacenId, ubicacionId, cantidadAnterior, cantidadPosterior));

        return movimiento;
    }
}
