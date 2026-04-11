using PlanTA.Compras.Domain.ValueObjects;
using PlanTA.SharedKernel;

namespace PlanTA.Compras.Domain.Entities;

public class Proveedor : SoftDeletableEntity<ProveedorId>
{
    private readonly List<ContactoProveedor> _contactos = [];
    private Proveedor() { }

    public string RazonSocial { get; private set; } = string.Empty;
    public string NIF { get; private set; } = string.Empty;
    public string? Direccion { get; private set; }
    public string? Ciudad { get; private set; }
    public string? CodigoPostal { get; private set; }
    public string? Pais { get; private set; }
    public string Email { get; private set; } = string.Empty;
    public string? Telefono { get; private set; }
    public string? Web { get; private set; }
    public CondicionesPago CondicionesPago { get; private set; } = default!;
    public bool Activo { get; private set; } = true;
    public Guid EmpresaId { get; private set; }

    public IReadOnlyList<ContactoProveedor> Contactos => _contactos.AsReadOnly();

    public static Proveedor Crear(
        string razonSocial,
        string nif,
        string email,
        CondicionesPago condicionesPago,
        Guid empresaId,
        string? direccion = null,
        string? ciudad = null,
        string? codigoPostal = null,
        string? pais = null,
        string? telefono = null,
        string? web = null)
    {
        return new Proveedor
        {
            Id = new ProveedorId(Guid.NewGuid()),
            RazonSocial = razonSocial,
            NIF = nif.Trim().ToUpperInvariant(),
            Email = email,
            CondicionesPago = condicionesPago,
            EmpresaId = empresaId,
            Direccion = direccion,
            Ciudad = ciudad,
            CodigoPostal = codigoPostal,
            Pais = pais,
            Telefono = telefono,
            Web = web
        };
    }

    public ContactoProveedor AgregarContacto(
        string nombre, string email, string? cargo = null, string? telefono = null, bool esPrincipal = false)
    {
        var contacto = ContactoProveedor.Crear(Id, nombre, email, cargo, telefono, esPrincipal);
        _contactos.Add(contacto);
        MarkUpdated();
        return contacto;
    }

    public void Actualizar(
        string razonSocial, string email, CondicionesPago condicionesPago,
        string? direccion, string? ciudad, string? codigoPostal, string? pais,
        string? telefono, string? web)
    {
        RazonSocial = razonSocial;
        Email = email;
        CondicionesPago = condicionesPago;
        Direccion = direccion;
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
