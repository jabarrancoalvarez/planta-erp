using PlanTA.SharedKernel;

namespace PlanTA.Compras.Domain.Entities;

public class ContactoProveedor : BaseEntity<ContactoProveedorId>
{
    private ContactoProveedor() { }

    public ProveedorId ProveedorId { get; private set; } = default!;
    public string Nombre { get; private set; } = string.Empty;
    public string? Cargo { get; private set; }
    public string Email { get; private set; } = string.Empty;
    public string? Telefono { get; private set; }
    public bool EsPrincipal { get; private set; }

    internal static ContactoProveedor Crear(
        ProveedorId proveedorId,
        string nombre,
        string email,
        string? cargo = null,
        string? telefono = null,
        bool esPrincipal = false)
    {
        return new ContactoProveedor
        {
            Id = new ContactoProveedorId(Guid.NewGuid()),
            ProveedorId = proveedorId,
            Nombre = nombre,
            Cargo = cargo,
            Email = email,
            Telefono = telefono,
            EsPrincipal = esPrincipal
        };
    }

    public void Actualizar(string nombre, string email, string? cargo, string? telefono, bool esPrincipal)
    {
        Nombre = nombre;
        Email = email;
        Cargo = cargo;
        Telefono = telefono;
        EsPrincipal = esPrincipal;
        MarkUpdated();
    }
}
