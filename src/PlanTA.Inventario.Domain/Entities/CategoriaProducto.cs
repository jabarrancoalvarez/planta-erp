using PlanTA.SharedKernel;

namespace PlanTA.Inventario.Domain.Entities;

public class CategoriaProducto : SoftDeletableEntity<CategoriaProductoId>
{
    private CategoriaProducto() { }

    public string Nombre { get; private set; } = string.Empty;
    public string? Descripcion { get; private set; }
    public CategoriaProductoId? CategoriaPadreId { get; private set; }
    public Guid EmpresaId { get; private set; }

    public static CategoriaProducto Crear(string nombre, Guid empresaId, string? descripcion = null, CategoriaProductoId? categoriaPadreId = null)
    {
        return new CategoriaProducto
        {
            Id = new CategoriaProductoId(Guid.NewGuid()),
            Nombre = nombre,
            Descripcion = descripcion,
            CategoriaPadreId = categoriaPadreId,
            EmpresaId = empresaId
        };
    }

    public void Actualizar(string nombre, string? descripcion)
    {
        Nombre = nombre;
        Descripcion = descripcion;
        MarkUpdated();
    }
}
