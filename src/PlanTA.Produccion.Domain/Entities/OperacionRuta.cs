using PlanTA.Produccion.Domain.Enums;
using PlanTA.Produccion.Domain.ValueObjects;
using PlanTA.SharedKernel;

namespace PlanTA.Produccion.Domain.Entities;

public class OperacionRuta : BaseEntity<OperacionRutaId>
{
    private OperacionRuta() { }

    public RutaProduccionId RutaProduccionId { get; private set; } = default!;
    public int Numero { get; private set; }
    public string Nombre { get; private set; } = string.Empty;
    public TipoOperacion TipoOperacion { get; private set; }
    public TiempoEstimado TiempoEstimado { get; private set; } = default!;
    public string CentroTrabajo { get; private set; } = string.Empty;
    public string? Instrucciones { get; private set; }

    internal static OperacionRuta Crear(
        RutaProduccionId rutaProduccionId,
        int numero,
        string nombre,
        TipoOperacion tipoOperacion,
        TiempoEstimado tiempoEstimado,
        string centroTrabajo,
        string? instrucciones = null)
    {
        return new OperacionRuta
        {
            Id = new OperacionRutaId(Guid.NewGuid()),
            RutaProduccionId = rutaProduccionId,
            Numero = numero,
            Nombre = nombre,
            TipoOperacion = tipoOperacion,
            TiempoEstimado = tiempoEstimado,
            CentroTrabajo = centroTrabajo,
            Instrucciones = instrucciones
        };
    }
}
