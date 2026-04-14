using PlanTA.SharedKernel;

namespace PlanTA.CRM.Domain.Errors;

public static class LeadErrors
{
    public static Error NotFound(Guid id) => Error.NotFound("Lead.NotFound", $"Lead '{id}' no encontrado");
}

public static class OportunidadErrors
{
    public static Error NotFound(Guid id) => Error.NotFound("Oportunidad.NotFound", $"Oportunidad '{id}' no encontrada");
    public static Error ImporteInvalido => Error.Validation("Oportunidad.ImporteInvalido", "El importe debe ser mayor que cero");
}
