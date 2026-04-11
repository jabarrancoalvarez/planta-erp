using PlanTA.Ventas.Domain.Enums;
using PlanTA.Ventas.Domain.Errors;
using PlanTA.Ventas.Domain.Events;
using PlanTA.SharedKernel;

namespace PlanTA.Ventas.Domain.Entities;

public class Expedicion : AggregateRoot<ExpedicionId>
{
    private readonly List<LineaExpedicion> _lineas = [];
    private Expedicion() { }

    public PedidoId PedidoId { get; private set; } = default!;
    public DateTimeOffset FechaExpedicion { get; private set; }
    public string? NumeroSeguimiento { get; private set; }
    public string? Transportista { get; private set; }
    public EstadoExpedicion EstadoExpedicion { get; private set; } = EstadoExpedicion.Pendiente;
    public string? Observaciones { get; private set; }
    public Guid EmpresaId { get; private set; }

    public IReadOnlyList<LineaExpedicion> Lineas => _lineas.AsReadOnly();

    public static Expedicion Crear(
        PedidoId pedidoId,
        Guid empresaId,
        string? numeroSeguimiento = null,
        string? transportista = null,
        string? observaciones = null)
    {
        return new Expedicion
        {
            Id = new ExpedicionId(Guid.NewGuid()),
            PedidoId = pedidoId,
            FechaExpedicion = DateTimeOffset.UtcNow,
            NumeroSeguimiento = numeroSeguimiento,
            Transportista = transportista,
            Observaciones = observaciones,
            EmpresaId = empresaId
        };
    }

    public LineaExpedicion AgregarLinea(
        LineaPedidoId lineaPedidoId,
        Guid productoId,
        decimal cantidad,
        Guid? loteOrigenId = null)
    {
        var linea = LineaExpedicion.Crear(Id, lineaPedidoId, productoId, cantidad, loteOrigenId);
        _lineas.Add(linea);
        MarkUpdated();
        return linea;
    }

    public Result<bool> IniciarPicking()
    {
        if (EstadoExpedicion != EstadoExpedicion.Pendiente)
            return Result<bool>.Failure(
                ExpedicionErrors.TransicionInvalida(EstadoExpedicion.ToString(), nameof(EstadoExpedicion.EnPicking)));

        EstadoExpedicion = EstadoExpedicion.EnPicking;
        MarkUpdated();
        return Result<bool>.Success(true);
    }

    public Result<bool> Empaquetar()
    {
        if (EstadoExpedicion != EstadoExpedicion.EnPicking)
            return Result<bool>.Failure(
                ExpedicionErrors.TransicionInvalida(EstadoExpedicion.ToString(), nameof(EstadoExpedicion.Empaquetada)));

        EstadoExpedicion = EstadoExpedicion.Empaquetada;
        MarkUpdated();
        AddDomainEvent(new ExpedicionPreparadaEvent(Id, PedidoId));
        return Result<bool>.Success(true);
    }

    public Result<bool> Enviar()
    {
        if (EstadoExpedicion != EstadoExpedicion.Empaquetada)
            return Result<bool>.Failure(
                ExpedicionErrors.TransicionInvalida(EstadoExpedicion.ToString(), nameof(EstadoExpedicion.Enviada)));

        EstadoExpedicion = EstadoExpedicion.Enviada;
        MarkUpdated();

        var lineasInfo = _lineas.Select(l => new LineaExpedicionInfo(
            l.ProductoId, l.Cantidad, l.LoteOrigenId)).ToList();
        AddDomainEvent(new ExpedicionEnviadaEvent(Id, PedidoId, lineasInfo));

        return Result<bool>.Success(true);
    }

    public Result<bool> Entregar()
    {
        if (EstadoExpedicion != EstadoExpedicion.Enviada)
            return Result<bool>.Failure(
                ExpedicionErrors.TransicionInvalida(EstadoExpedicion.ToString(), nameof(EstadoExpedicion.Entregada)));

        EstadoExpedicion = EstadoExpedicion.Entregada;
        MarkUpdated();
        return Result<bool>.Success(true);
    }
}
