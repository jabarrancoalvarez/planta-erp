using PlanTA.Produccion.Domain.Enums;
using PlanTA.Produccion.Domain.ValueObjects;
using PlanTA.SharedKernel;

namespace PlanTA.Produccion.Domain.Entities;

public class RutaProduccion : SoftDeletableEntity<RutaProduccionId>
{
    private readonly List<OperacionRuta> _operaciones = [];
    private RutaProduccion() { }

    public Guid ProductoId { get; private set; }
    public string Nombre { get; private set; } = string.Empty;
    public string? Descripcion { get; private set; }
    public bool Activa { get; private set; } = true;
    public Guid EmpresaId { get; private set; }

    public IReadOnlyList<OperacionRuta> Operaciones => _operaciones.AsReadOnly();

    public static RutaProduccion Crear(
        Guid productoId,
        string nombre,
        Guid empresaId,
        string? descripcion = null)
    {
        return new RutaProduccion
        {
            Id = new RutaProduccionId(Guid.NewGuid()),
            ProductoId = productoId,
            Nombre = nombre,
            Descripcion = descripcion,
            EmpresaId = empresaId
        };
    }

    public OperacionRuta AgregarOperacion(
        int numero,
        string nombre,
        TipoOperacion tipoOperacion,
        TiempoEstimado tiempoEstimado,
        string centroTrabajo,
        string? instrucciones = null)
    {
        var operacion = OperacionRuta.Crear(
            Id, numero, nombre, tipoOperacion, tiempoEstimado, centroTrabajo, instrucciones);
        _operaciones.Add(operacion);
        MarkUpdated();
        return operacion;
    }

    public void Desactivar()
    {
        Activa = false;
        MarkUpdated();
    }
}
