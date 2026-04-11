using PlanTA.Inventario.Domain.Enums;
using PlanTA.Inventario.Domain.Events;
using PlanTA.Inventario.Domain.ValueObjects;
using PlanTA.SharedKernel;

namespace PlanTA.Inventario.Domain.Entities;

public class Producto : SoftDeletableEntity<ProductoId>
{
    private Producto() { }

    public SKU SKU { get; private set; } = default!;
    public string Nombre { get; private set; } = string.Empty;
    public string? Descripcion { get; private set; }
    public TipoProducto Tipo { get; private set; }
    public UnidadMedida UnidadMedida { get; private set; }
    public CategoriaProductoId? CategoriaId { get; private set; }
    public decimal PrecioCompra { get; private set; }
    public decimal PrecioVenta { get; private set; }
    public decimal PesoKg { get; private set; }
    public string? CodigoBarras { get; private set; }
    public string? ImagenUrl { get; private set; }
    public bool Activo { get; private set; } = true;
    public Guid EmpresaId { get; private set; }

    public static Producto Crear(
        string sku, string nombre, TipoProducto tipo, UnidadMedida unidadMedida,
        Guid empresaId, string? descripcion = null, CategoriaProductoId? categoriaId = null,
        decimal precioCompra = 0, decimal precioVenta = 0)
    {
        var producto = new Producto
        {
            Id = new ProductoId(Guid.NewGuid()),
            SKU = SKU.Create(sku),
            Nombre = nombre,
            Descripcion = descripcion,
            Tipo = tipo,
            UnidadMedida = unidadMedida,
            CategoriaId = categoriaId,
            PrecioCompra = precioCompra,
            PrecioVenta = precioVenta,
            EmpresaId = empresaId
        };

        producto.AddDomainEvent(new ProductoCreadoEvent(producto.Id, producto.SKU.Value));
        return producto;
    }

    public void Actualizar(string nombre, string? descripcion, decimal precioCompra, decimal precioVenta, CategoriaProductoId? categoriaId)
    {
        Nombre = nombre;
        Descripcion = descripcion;
        PrecioCompra = precioCompra;
        PrecioVenta = precioVenta;
        CategoriaId = categoriaId;
        MarkUpdated();
    }

    public void CambiarImagen(string url)
    {
        ImagenUrl = url;
        MarkUpdated();
    }

    public void Desactivar()
    {
        Activo = false;
        MarkUpdated();
    }

    public void Activar()
    {
        Activo = true;
        MarkUpdated();
    }
}
