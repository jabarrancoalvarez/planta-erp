using PlanTA.SharedKernel;

namespace PlanTA.Ventas.Domain.Entities;

public class Cliente : SoftDeletableEntity<ClienteId>
{
    private readonly List<ContactoCliente> _contactos = [];
    private Cliente() { }

    public string RazonSocial { get; private set; } = string.Empty;
    public string NIF { get; private set; } = string.Empty;
    public string? DireccionEnvio { get; private set; }
    public string? DireccionFacturacion { get; private set; }
    public string? Ciudad { get; private set; }
    public string? CodigoPostal { get; private set; }
    public string? Pais { get; private set; }
    public string Email { get; private set; } = string.Empty;
    public string? Telefono { get; private set; }
    public string? Web { get; private set; }
    public bool Activo { get; private set; } = true;
    public Guid EmpresaId { get; private set; }

    public IReadOnlyList<ContactoCliente> Contactos => _contactos.AsReadOnly();

    public static Cliente Crear(
        string razonSocial,
        string nif,
        string email,
        Guid empresaId,
        string? direccionEnvio = null,
        string? direccionFacturacion = null,
        string? ciudad = null,
        string? codigoPostal = null,
        string? pais = null,
        string? telefono = null,
        string? web = null)
    {
        return new Cliente
        {
            Id = new ClienteId(Guid.NewGuid()),
            RazonSocial = razonSocial,
            NIF = nif.Trim().ToUpperInvariant(),
            Email = email,
            EmpresaId = empresaId,
            DireccionEnvio = direccionEnvio,
            DireccionFacturacion = direccionFacturacion,
            Ciudad = ciudad,
            CodigoPostal = codigoPostal,
            Pais = pais,
            Telefono = telefono,
            Web = web
        };
    }

    public ContactoCliente AgregarContacto(
        string nombre, string email, string? cargo = null, string? telefono = null, bool esPrincipal = false)
    {
        var contacto = ContactoCliente.Crear(Id, nombre, email, cargo, telefono, esPrincipal);
        _contactos.Add(contacto);
        MarkUpdated();
        return contacto;
    }

    public void Actualizar(
        string razonSocial, string email,
        string? direccionEnvio, string? direccionFacturacion,
        string? ciudad, string? codigoPostal, string? pais,
        string? telefono, string? web)
    {
        RazonSocial = razonSocial;
        Email = email;
        DireccionEnvio = direccionEnvio;
        DireccionFacturacion = direccionFacturacion;
        Ciudad = ciudad;
        CodigoPostal = codigoPostal;
        Pais = pais;
        Telefono = telefono;
        Web = web;
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
