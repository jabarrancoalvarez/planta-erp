using PlanTA.SharedKernel;

namespace PlanTA.Inventario.Domain.Entities;

public class Almacen : SoftDeletableEntity<AlmacenId>
{
    private readonly List<Ubicacion> _ubicaciones = [];
    private Almacen() { }

    public string Nombre { get; private set; } = string.Empty;
    public string? Direccion { get; private set; }
    public string? Descripcion { get; private set; }
    public bool EsPrincipal { get; private set; }
    public Guid EmpresaId { get; private set; }

    public IReadOnlyList<Ubicacion> Ubicaciones => _ubicaciones.AsReadOnly();

    public static Almacen Crear(string nombre, Guid empresaId, string? direccion = null, string? descripcion = null, bool esPrincipal = false)
    {
        return new Almacen
        {
            Id = new AlmacenId(Guid.NewGuid()),
            Nombre = nombre,
            Direccion = direccion,
            Descripcion = descripcion,
            EsPrincipal = esPrincipal,
            EmpresaId = empresaId
        };
    }

    public Ubicacion AgregarUbicacion(string pasillo, string estante, string nivel, string? nombre = null, int capacidadMaxima = 0)
    {
        var ubicacion = Ubicacion.Crear(Id, pasillo, estante, nivel, EmpresaId, nombre, capacidadMaxima);
        _ubicaciones.Add(ubicacion);
        return ubicacion;
    }

    public void Actualizar(string nombre, string? direccion, string? descripcion)
    {
        Nombre = nombre;
        Direccion = direccion;
        Descripcion = descripcion;
        MarkUpdated();
    }

    public void MarcarComoPrincipal()
    {
        EsPrincipal = true;
        MarkUpdated();
    }
}
