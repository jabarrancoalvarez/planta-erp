using PlanTA.Compras.Domain.Enums;
using PlanTA.Compras.Domain.Errors;
using PlanTA.Compras.Domain.Events;
using PlanTA.SharedKernel;

namespace PlanTA.Compras.Domain.Entities;

public class Recepcion : AggregateRoot<RecepcionId>
{
    private readonly List<LineaRecepcion> _lineas = [];
    private Recepcion() { }

    public OrdenCompraId OrdenCompraId { get; private set; } = default!;
    public DateTimeOffset FechaRecepcion { get; private set; }
    public string? NumeroAlbaran { get; private set; }
    public EstadoRecepcion EstadoRecepcion { get; private set; } = EstadoRecepcion.Pendiente;
    public string? Observaciones { get; private set; }
    public Guid EmpresaId { get; private set; }

    public IReadOnlyList<LineaRecepcion> Lineas => _lineas.AsReadOnly();

    public static Recepcion Crear(
        OrdenCompraId ordenCompraId,
        Guid empresaId,
        string? numeroAlbaran = null,
        string? observaciones = null)
    {
        var recepcion = new Recepcion
        {
            Id = new RecepcionId(Guid.NewGuid()),
            OrdenCompraId = ordenCompraId,
            FechaRecepcion = DateTimeOffset.UtcNow,
            NumeroAlbaran = numeroAlbaran,
            Observaciones = observaciones,
            EmpresaId = empresaId
        };

        return recepcion;
    }

    public LineaRecepcion AgregarLinea(
        LineaOrdenCompraId lineaOrdenCompraId,
        Guid productoId,
        decimal cantidadRecibida,
        Guid? loteId = null,
        Guid? ubicacionDestinoId = null)
    {
        var linea = LineaRecepcion.Crear(Id, lineaOrdenCompraId, productoId, cantidadRecibida, loteId, ubicacionDestinoId);
        _lineas.Add(linea);
        MarkUpdated();
        return linea;
    }

    public Result<bool> Inspeccionar()
    {
        if (EstadoRecepcion != EstadoRecepcion.Pendiente)
            return Result<bool>.Failure(
                RecepcionErrors.TransicionInvalida(EstadoRecepcion.ToString(), nameof(EstadoRecepcion.EnInspeccion)));

        EstadoRecepcion = EstadoRecepcion.EnInspeccion;
        MarkUpdated();
        return Result<bool>.Success(true);
    }

    public Result<bool> Aceptar()
    {
        if (EstadoRecepcion is not (EstadoRecepcion.Pendiente or EstadoRecepcion.EnInspeccion))
            return Result<bool>.Failure(
                RecepcionErrors.TransicionInvalida(EstadoRecepcion.ToString(), nameof(EstadoRecepcion.Aceptada)));

        EstadoRecepcion = EstadoRecepcion.Aceptada;
        MarkUpdated();
        AddDomainEvent(new RecepcionAceptadaEvent(Id));

        var lineasInfo = _lineas.Select(l => new LineaRecepcionInfo(
            l.ProductoId, l.CantidadRecibida, l.LoteId, l.UbicacionDestinoId)).ToList();
        AddDomainEvent(new RecepcionRegistradaEvent(Id, OrdenCompraId, lineasInfo));

        return Result<bool>.Success(true);
    }

    public Result<bool> Rechazar(string motivo)
    {
        if (EstadoRecepcion is not (EstadoRecepcion.Pendiente or EstadoRecepcion.EnInspeccion))
            return Result<bool>.Failure(
                RecepcionErrors.TransicionInvalida(EstadoRecepcion.ToString(), nameof(EstadoRecepcion.Rechazada)));

        EstadoRecepcion = EstadoRecepcion.Rechazada;
        Observaciones = motivo;
        MarkUpdated();
        return Result<bool>.Success(true);
    }
}
