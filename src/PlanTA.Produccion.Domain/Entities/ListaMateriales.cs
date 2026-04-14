using PlanTA.SharedKernel;

namespace PlanTA.Produccion.Domain.Entities;

public class ListaMateriales : SoftDeletableEntity<ListaMaterialesId>
{
    private readonly List<LineaBOM> _lineas = [];
    private ListaMateriales() { }

    public Guid ProductoId { get; private set; }
    public string Nombre { get; private set; } = string.Empty;
    public string? Descripcion { get; private set; }
    public int VersionBOM { get; private set; } = 1;
    public bool Activo { get; private set; } = true;
    public Guid EmpresaId { get; private set; }

    public IReadOnlyList<LineaBOM> Lineas => _lineas.AsReadOnly();

    public static ListaMateriales Crear(
        Guid productoId,
        string nombre,
        Guid empresaId,
        string? descripcion = null)
    {
        return new ListaMateriales
        {
            Id = new ListaMaterialesId(Guid.NewGuid()),
            ProductoId = productoId,
            Nombre = nombre,
            Descripcion = descripcion,
            EmpresaId = empresaId
        };
    }

    public LineaBOM AgregarLinea(
        Guid componenteProductoId,
        decimal cantidad,
        string unidadMedida,
        decimal merma = 0,
        int? orden = null)
    {
        var ordenFinal = orden ?? (_lineas.Count + 1);
        var linea = LineaBOM.Crear(Id, componenteProductoId, cantidad, unidadMedida, merma, ordenFinal);
        _lineas.Add(linea);
        MarkUpdated();
        return linea;
    }

    public void RemoverLinea(LineaBOMId lineaId)
    {
        var linea = _lineas.FirstOrDefault(l => l.Id == lineaId);
        if (linea is not null)
        {
            _lineas.Remove(linea);
            MarkUpdated();
        }
    }

    public void Actualizar(string nombre, string? descripcion)
    {
        Nombre = nombre.Trim();
        Descripcion = descripcion;
        MarkUpdated();
    }

    public void Desactivar()
    {
        Activo = false;
        MarkUpdated();
    }

    public void IncrementarVersion()
    {
        VersionBOM++;
        MarkUpdated();
    }
}
