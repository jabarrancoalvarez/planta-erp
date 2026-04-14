using PlanTA.CRM.Domain.Enums;
using PlanTA.SharedKernel;

namespace PlanTA.CRM.Domain.Entities;

public class Lead : SoftDeletableEntity<LeadId>
{
    private Lead() { }

    public string Nombre { get; private set; } = string.Empty;
    public string? Empresa { get; private set; }
    public string? Email { get; private set; }
    public string? Telefono { get; private set; }
    public OrigenLead Origen { get; private set; }
    public EstadoLead Estado { get; private set; } = EstadoLead.Nuevo;
    public string? Notas { get; private set; }
    public Guid? AsignadoAUserId { get; private set; }
    public Guid EmpresaId { get; private set; }

    public static Lead Crear(
        string nombre, OrigenLead origen, Guid empresaId,
        string? empresa = null, string? email = null, string? telefono = null,
        string? notas = null, Guid? asignadoAUserId = null)
        => new()
        {
            Id = new LeadId(Guid.NewGuid()),
            Nombre = nombre.Trim(),
            Empresa = empresa,
            Email = email,
            Telefono = telefono,
            Origen = origen,
            Notas = notas,
            AsignadoAUserId = asignadoAUserId,
            EmpresaId = empresaId
        };

    public void CambiarEstado(EstadoLead estado) { Estado = estado; MarkUpdated(); }
    public void Asignar(Guid userId) { AsignadoAUserId = userId; MarkUpdated(); }

    public void Editar(string nombre, string? empresa, string? email, string? telefono, string? notas)
    {
        Nombre = nombre.Trim();
        Empresa = empresa;
        Email = email;
        Telefono = telefono;
        Notas = notas;
        MarkUpdated();
    }
}
