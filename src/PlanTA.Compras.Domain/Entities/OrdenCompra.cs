using PlanTA.Compras.Domain.Enums;
using PlanTA.Compras.Domain.Errors;
using PlanTA.Compras.Domain.Events;
using PlanTA.SharedKernel;

namespace PlanTA.Compras.Domain.Entities;

public class OrdenCompra : SoftDeletableEntity<OrdenCompraId>
{
    private readonly List<LineaOrdenCompra> _lineas = [];
    private OrdenCompra() { }

    public string Codigo { get; private set; } = string.Empty;
    public ProveedorId ProveedorId { get; private set; } = default!;
    public DateTimeOffset FechaEmision { get; private set; }
    public DateTimeOffset? FechaEntregaEstimada { get; private set; }
    public EstadoOC EstadoOC { get; private set; } = EstadoOC.Borrador;
    public string? Observaciones { get; private set; }
    public Guid EmpresaId { get; private set; }

    public IReadOnlyList<LineaOrdenCompra> Lineas => _lineas.AsReadOnly();

    public decimal Total => _lineas.Sum(l => l.Total);

    public static OrdenCompra Crear(
        string codigo,
        ProveedorId proveedorId,
        Guid empresaId,
        DateTimeOffset? fechaEntregaEstimada = null,
        string? observaciones = null)
    {
        return new OrdenCompra
        {
            Id = new OrdenCompraId(Guid.NewGuid()),
            Codigo = codigo.Trim().ToUpperInvariant(),
            ProveedorId = proveedorId,
            FechaEmision = DateTimeOffset.UtcNow,
            FechaEntregaEstimada = fechaEntregaEstimada,
            Observaciones = observaciones,
            EmpresaId = empresaId
        };
    }

    public Result<bool> Editar(DateTimeOffset? fechaEntregaEstimada, string? observaciones)
    {
        if (EstadoOC is EstadoOC.Recibida or EstadoOC.Cancelada)
            return Result<bool>.Failure(
                OrdenCompraErrors.TransicionInvalida(EstadoOC.ToString(), "Editar"));

        FechaEntregaEstimada = fechaEntregaEstimada;
        Observaciones = observaciones;
        MarkUpdated();
        return Result<bool>.Success(true);
    }

    public Result<LineaOrdenCompra> AgregarLinea(
        Guid productoId, string descripcion, decimal cantidad, decimal precioUnitario)
    {
        if (EstadoOC != EstadoOC.Borrador)
            return Result<LineaOrdenCompra>.Failure(OrdenCompraErrors.NoBorrador(Id.Value));

        var linea = LineaOrdenCompra.Crear(Id, productoId, descripcion, cantidad, precioUnitario);
        _lineas.Add(linea);
        MarkUpdated();
        return Result<LineaOrdenCompra>.Success(linea);
    }

    public Result<bool> Enviar()
    {
        if (EstadoOC != EstadoOC.Borrador)
            return Result<bool>.Failure(
                OrdenCompraErrors.TransicionInvalida(EstadoOC.ToString(), nameof(EstadoOC.Enviada)));

        if (_lineas.Count == 0)
            return Result<bool>.Failure(OrdenCompraErrors.SinLineas(Id.Value));

        EstadoOC = EstadoOC.Enviada;
        MarkUpdated();
        AddDomainEvent(new OCEnviadaEvent(Id, ProveedorId));
        return Result<bool>.Success(true);
    }

    public Result<bool> RecibirParcialmente()
    {
        if (EstadoOC is not (EstadoOC.Enviada or EstadoOC.ParcialmenteRecibida))
            return Result<bool>.Failure(
                OrdenCompraErrors.TransicionInvalida(EstadoOC.ToString(), nameof(EstadoOC.ParcialmenteRecibida)));

        EstadoOC = EstadoOC.ParcialmenteRecibida;
        MarkUpdated();
        return Result<bool>.Success(true);
    }

    public Result<bool> RecibirCompleta()
    {
        if (EstadoOC is not (EstadoOC.Enviada or EstadoOC.ParcialmenteRecibida))
            return Result<bool>.Failure(
                OrdenCompraErrors.TransicionInvalida(EstadoOC.ToString(), nameof(EstadoOC.Recibida)));

        EstadoOC = EstadoOC.Recibida;
        MarkUpdated();
        AddDomainEvent(new OCCompletadaEvent(Id));
        return Result<bool>.Success(true);
    }

    public Result<bool> Cancelar(string motivo)
    {
        if (EstadoOC is EstadoOC.Recibida or EstadoOC.Cancelada)
            return Result<bool>.Failure(
                OrdenCompraErrors.TransicionInvalida(EstadoOC.ToString(), nameof(EstadoOC.Cancelada)));

        EstadoOC = EstadoOC.Cancelada;
        Observaciones = motivo;
        MarkUpdated();
        return Result<bool>.Success(true);
    }

    public bool TodasLineasRecibidas()
    {
        return _lineas.All(l => l.CantidadRecibida >= l.Cantidad);
    }

    public bool AlgunaLineaParcialmenteRecibida()
    {
        return _lineas.Any(l => l.CantidadRecibida > 0 && l.CantidadRecibida < l.Cantidad);
    }
}
