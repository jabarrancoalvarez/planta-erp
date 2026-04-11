using PlanTA.SharedKernel;

namespace PlanTA.Ventas.Domain.Entities;

public class ContactoCliente : BaseEntity<ContactoClienteId>
{
    private ContactoCliente() { }

    public ClienteId ClienteId { get; private set; } = default!;
    public string Nombre { get; private set; } = string.Empty;
    public string? Cargo { get; private set; }
    public string Email { get; private set; } = string.Empty;
    public string? Telefono { get; private set; }
    public bool EsPrincipal { get; private set; }

    internal static ContactoCliente Crear(
        ClienteId clienteId,
        string nombre,
        string email,
        string? cargo = null,
        string? telefono = null,
        bool esPrincipal = false)
    {
        return new ContactoCliente
        {
            Id = new ContactoClienteId(Guid.NewGuid()),
            ClienteId = clienteId,
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
