using PlanTA.Ventas.Domain.Enums;
using PlanTA.Ventas.Domain.Errors;
using PlanTA.Ventas.Domain.Events;
using PlanTA.SharedKernel;

namespace PlanTA.Ventas.Domain.Entities;

public class Pedido : SoftDeletableEntity<PedidoId>
{
    private readonly List<LineaPedido> _lineas = [];
    private Pedido() { }

    public string Codigo { get; private set; } = string.Empty;
    public ClienteId ClienteId { get; private set; } = default!;
    public DateTimeOffset FechaEmision { get; private set; }
    public DateTimeOffset? FechaEntregaEstimada { get; private set; }
    public EstadoPedido EstadoPedido { get; private set; } = EstadoPedido.Borrador;
    public string? DireccionEntrega { get; private set; }
    public string? Observaciones { get; private set; }
    public Guid EmpresaId { get; private set; }

    public IReadOnlyList<LineaPedido> Lineas => _lineas.AsReadOnly();

    public decimal Total => _lineas.Sum(l => l.Total);

    public static Pedido Crear(
        string codigo,
        ClienteId clienteId,
        Guid empresaId,
        DateTimeOffset? fechaEntregaEstimada = null,
        string? direccionEntrega = null,
        string? observaciones = null)
    {
        return new Pedido
        {
            Id = new PedidoId(Guid.NewGuid()),
            Codigo = codigo.Trim().ToUpperInvariant(),
            ClienteId = clienteId,
            FechaEmision = DateTimeOffset.UtcNow,
            FechaEntregaEstimada = fechaEntregaEstimada,
            DireccionEntrega = direccionEntrega,
            Observaciones = observaciones,
            EmpresaId = empresaId
        };
    }

    public Result<bool> Editar(DateTimeOffset? fechaEntregaEstimada, string? direccionEntrega, string? observaciones)
    {
        if (EstadoPedido is EstadoPedido.Entregado or EstadoPedido.Cancelado)
            return Result<bool>.Failure(
                PedidoErrors.TransicionInvalida(EstadoPedido.ToString(), "Editar"));

        FechaEntregaEstimada = fechaEntregaEstimada;
        DireccionEntrega = direccionEntrega;
        Observaciones = observaciones;
        MarkUpdated();
        return Result<bool>.Success(true);
    }

    public Result<LineaPedido> AgregarLinea(
        Guid productoId, string descripcion, decimal cantidad, decimal precioUnitario, decimal descuento = 0)
    {
        if (EstadoPedido != EstadoPedido.Borrador)
            return Result<LineaPedido>.Failure(PedidoErrors.NoBorrador(Id.Value));

        var linea = LineaPedido.Crear(Id, productoId, descripcion, cantidad, precioUnitario, descuento);
        _lineas.Add(linea);
        MarkUpdated();
        return Result<LineaPedido>.Success(linea);
    }

    public Result<bool> Confirmar()
    {
        if (EstadoPedido != EstadoPedido.Borrador)
            return Result<bool>.Failure(
                PedidoErrors.TransicionInvalida(EstadoPedido.ToString(), nameof(EstadoPedido.Confirmado)));

        if (_lineas.Count == 0)
            return Result<bool>.Failure(PedidoErrors.SinLineas(Id.Value));

        EstadoPedido = EstadoPedido.Confirmado;
        MarkUpdated();
        AddDomainEvent(new PedidoConfirmadoEvent(Id, ClienteId));
        return Result<bool>.Success(true);
    }

    public Result<bool> PrepararEnvio()
    {
        if (EstadoPedido != EstadoPedido.Confirmado)
            return Result<bool>.Failure(
                PedidoErrors.TransicionInvalida(EstadoPedido.ToString(), nameof(EstadoPedido.EnPreparacion)));

        EstadoPedido = EstadoPedido.EnPreparacion;
        MarkUpdated();
        return Result<bool>.Success(true);
    }

    public Result<bool> EnviarParcialmente()
    {
        if (EstadoPedido is not (EstadoPedido.EnPreparacion or EstadoPedido.Confirmado or EstadoPedido.ParcialmenteEnviado))
            return Result<bool>.Failure(
                PedidoErrors.TransicionInvalida(EstadoPedido.ToString(), nameof(EstadoPedido.ParcialmenteEnviado)));

        EstadoPedido = EstadoPedido.ParcialmenteEnviado;
        MarkUpdated();
        return Result<bool>.Success(true);
    }

    public Result<bool> EnviarCompleto()
    {
        if (EstadoPedido is not (EstadoPedido.EnPreparacion or EstadoPedido.Confirmado or EstadoPedido.ParcialmenteEnviado))
            return Result<bool>.Failure(
                PedidoErrors.TransicionInvalida(EstadoPedido.ToString(), nameof(EstadoPedido.Enviado)));

        EstadoPedido = EstadoPedido.Enviado;
        MarkUpdated();
        return Result<bool>.Success(true);
    }

    public Result<bool> Entregar()
    {
        if (EstadoPedido != EstadoPedido.Enviado)
            return Result<bool>.Failure(
                PedidoErrors.TransicionInvalida(EstadoPedido.ToString(), nameof(EstadoPedido.Entregado)));

        EstadoPedido = EstadoPedido.Entregado;
        MarkUpdated();
        AddDomainEvent(new PedidoEntregadoEvent(Id));
        return Result<bool>.Success(true);
    }

    public Result<bool> Cancelar(string motivo)
    {
        if (EstadoPedido is EstadoPedido.Entregado or EstadoPedido.Cancelado)
            return Result<bool>.Failure(
                PedidoErrors.TransicionInvalida(EstadoPedido.ToString(), nameof(EstadoPedido.Cancelado)));

        EstadoPedido = EstadoPedido.Cancelado;
        Observaciones = motivo;
        MarkUpdated();
        return Result<bool>.Success(true);
    }

    public bool TodasLineasEnviadas()
    {
        return _lineas.All(l => l.CantidadEnviada >= l.Cantidad);
    }

    public bool AlgunaLineaParcialmenteEnviada()
    {
        return _lineas.Any(l => l.CantidadEnviada > 0 && l.CantidadEnviada < l.Cantidad);
    }
}
