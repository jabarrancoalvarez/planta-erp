using PlanTA.Inventario.Domain.Enums;
using PlanTA.Inventario.Domain.Events;
using PlanTA.Inventario.Domain.ValueObjects;
using PlanTA.SharedKernel;

namespace PlanTA.Inventario.Domain.Entities;

public class Lote : SoftDeletableEntity<LoteId>
{
    private Lote() { }

    public CodigoLote Codigo { get; private set; } = default!;
    public ProductoId ProductoId { get; private set; } = default!;
    public decimal CantidadInicial { get; private set; }
    public decimal CantidadActual { get; private set; }
    public EstadoLote Estado { get; private set; } = EstadoLote.Activo;
    public DateTimeOffset? FechaCaducidad { get; private set; }
    public DateTimeOffset FechaRecepcion { get; private set; }
    public string? Origen { get; private set; }
    public string? Notas { get; private set; }
    public Guid EmpresaId { get; private set; }

    public static Lote Crear(
        ProductoId productoId, decimal cantidad, Guid empresaId,
        string? codigoLote = null, DateTimeOffset? fechaCaducidad = null,
        string? origen = null, string? notas = null)
    {
        var lote = new Lote
        {
            Id = new LoteId(Guid.NewGuid()),
            Codigo = codigoLote is not null ? CodigoLote.Create(codigoLote) : CodigoLote.Generate(),
            ProductoId = productoId,
            CantidadInicial = cantidad,
            CantidadActual = cantidad,
            FechaCaducidad = fechaCaducidad,
            FechaRecepcion = DateTimeOffset.UtcNow,
            Origen = origen,
            Notas = notas,
            EmpresaId = empresaId
        };

        lote.AddDomainEvent(new LoteCreadoEvent(lote.Id, productoId, cantidad));
        return lote;
    }

    public void ReducirCantidad(decimal cantidad)
    {
        if (cantidad > CantidadActual)
            throw new InvalidOperationException("No se puede reducir más de la cantidad actual del lote");

        CantidadActual -= cantidad;
        if (CantidadActual == 0)
            Estado = EstadoLote.Agotado;

        MarkUpdated();
    }

    public void AumentarCantidad(decimal cantidad)
    {
        CantidadActual += cantidad;
        if (Estado == EstadoLote.Agotado)
            Estado = EstadoLote.Activo;

        MarkUpdated();
    }

    public void Bloquear(string motivo)
    {
        Estado = EstadoLote.Bloqueado;
        Notas = motivo;
        MarkUpdated();
        AddDomainEvent(new LoteBloqueadoEvent(Id, motivo));
    }

    public void PonerEnCuarentena()
    {
        Estado = EstadoLote.EnCuarentena;
        MarkUpdated();
    }

    public void Liberar()
    {
        Estado = CantidadActual > 0 ? EstadoLote.Activo : EstadoLote.Agotado;
        MarkUpdated();
    }
}
