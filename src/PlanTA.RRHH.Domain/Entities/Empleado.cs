using PlanTA.SharedKernel;

namespace PlanTA.RRHH.Domain.Entities;

public class Empleado : SoftDeletableEntity<EmpleadoId>
{
    private Empleado() { }

    public string Codigo { get; private set; } = string.Empty;
    public string Nombre { get; private set; } = string.Empty;
    public string Apellidos { get; private set; } = string.Empty;
    public string DNI { get; private set; } = string.Empty;
    public string? Email { get; private set; }
    public string? Telefono { get; private set; }
    public string Puesto { get; private set; } = string.Empty;
    public string? Departamento { get; private set; }
    public DateTimeOffset FechaAlta { get; private set; }
    public DateTimeOffset? FechaBaja { get; private set; }
    public decimal CosteHoraEstandar { get; private set; }
    public int DiasVacacionesAnuales { get; private set; } = 22;
    public Guid? UserId { get; private set; }
    public Guid EmpresaId { get; private set; }

    public string NombreCompleto => $"{Nombre} {Apellidos}".Trim();

    public static Empleado Crear(
        string codigo, string nombre, string apellidos, string dni, string puesto,
        Guid empresaId, DateTimeOffset? fechaAlta = null, decimal costeHoraEstandar = 0,
        string? email = null, string? telefono = null, string? departamento = null,
        Guid? userId = null)
        => new()
        {
            Id = new EmpleadoId(Guid.NewGuid()),
            Codigo = codigo.Trim().ToUpperInvariant(),
            Nombre = nombre.Trim(),
            Apellidos = apellidos.Trim(),
            DNI = dni.Trim().ToUpperInvariant(),
            Email = email,
            Telefono = telefono,
            Puesto = puesto,
            Departamento = departamento,
            FechaAlta = fechaAlta ?? DateTimeOffset.UtcNow,
            CosteHoraEstandar = costeHoraEstandar,
            UserId = userId,
            EmpresaId = empresaId
        };

    public void DarBaja(DateTimeOffset fecha) { FechaBaja = fecha; MarkUpdated(); }
    public void ActualizarCoste(decimal coste) { CosteHoraEstandar = coste; MarkUpdated(); }

    public void Editar(
        string nombre, string apellidos, string puesto,
        string? email, string? telefono, string? departamento,
        decimal costeHoraEstandar, int diasVacacionesAnuales)
    {
        Nombre = nombre.Trim();
        Apellidos = apellidos.Trim();
        Puesto = puesto;
        Email = email;
        Telefono = telefono;
        Departamento = departamento;
        CosteHoraEstandar = costeHoraEstandar;
        DiasVacacionesAnuales = diasVacacionesAnuales;
        MarkUpdated();
    }
}
