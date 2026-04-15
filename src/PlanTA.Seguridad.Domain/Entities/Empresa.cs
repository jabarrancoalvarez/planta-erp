using PlanTA.SharedKernel;

namespace PlanTA.Seguridad.Domain.Entities;

public record EmpresaId(Guid Value) : EntityId(Value);

public class Empresa : SoftDeletableEntity<EmpresaId>
{
    private Empresa() { }

    public string Nombre { get; private set; } = string.Empty;
    public string? CIF { get; private set; }
    public string? Direccion { get; private set; }
    public string? Telefono { get; private set; }
    public string? Email { get; private set; }
    public string? LogoUrl { get; private set; }
    public string ZonaHoraria { get; private set; } = "Europe/Madrid";
    public string Moneda { get; private set; } = "EUR";
    public DateTimeOffset? TrialHasta { get; private set; }
    public bool OnboardingCompletado { get; private set; }

    public static Empresa Crear(string nombre, string? cif = null, string? email = null, int? trialDias = null)
    {
        return new Empresa
        {
            Id = new EmpresaId(Guid.NewGuid()),
            Nombre = nombre,
            CIF = cif,
            Email = email,
            TrialHasta = trialDias.HasValue ? DateTimeOffset.UtcNow.AddDays(trialDias.Value) : null,
            OnboardingCompletado = false
        };
    }

    public void CompletarOnboarding()
    {
        OnboardingCompletado = true;
        MarkUpdated();
    }

    public void ActualizarDatos(string nombre, string? cif, string? direccion, string? telefono, string? email)
    {
        Nombre = nombre;
        CIF = cif;
        Direccion = direccion;
        Telefono = telefono;
        Email = email;
        MarkUpdated();
    }

    public void CambiarLogo(string logoUrl)
    {
        LogoUrl = logoUrl;
        MarkUpdated();
    }
}
