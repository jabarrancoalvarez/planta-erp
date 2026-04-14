using PlanTA.CRM.Domain.Enums;
using PlanTA.CRM.Domain.Errors;
using PlanTA.SharedKernel;

namespace PlanTA.CRM.Domain.Entities;

public class Oportunidad : SoftDeletableEntity<OportunidadId>
{
    private Oportunidad() { }

    public string Titulo { get; private set; } = string.Empty;
    public Guid? ClienteId { get; private set; }
    public Guid? LeadId { get; private set; }
    public FaseOportunidad Fase { get; private set; } = FaseOportunidad.Prospecto;
    public decimal ImporteEstimado { get; private set; }
    public int ProbabilidadPct { get; private set; } = 10;
    public DateTimeOffset? FechaCierreEstimada { get; private set; }
    public string? Descripcion { get; private set; }
    public Guid? PropietarioUserId { get; private set; }
    public Guid EmpresaId { get; private set; }

    public decimal ValorPonderado => ImporteEstimado * ProbabilidadPct / 100m;

    public static Result<Oportunidad> Crear(
        string titulo, decimal importe, Guid empresaId,
        Guid? clienteId = null, Guid? leadId = null,
        DateTimeOffset? fechaCierreEstimada = null, string? descripcion = null,
        Guid? propietarioUserId = null)
    {
        if (importe <= 0)
            return Result<Oportunidad>.Failure(OportunidadErrors.ImporteInvalido);

        return Result<Oportunidad>.Success(new Oportunidad
        {
            Id = new OportunidadId(Guid.NewGuid()),
            Titulo = titulo.Trim(),
            ImporteEstimado = importe,
            ClienteId = clienteId,
            LeadId = leadId,
            FechaCierreEstimada = fechaCierreEstimada,
            Descripcion = descripcion,
            PropietarioUserId = propietarioUserId,
            EmpresaId = empresaId
        });
    }

    public Result<bool> Editar(string titulo, decimal importeEstimado, DateTimeOffset? fechaCierreEstimada, string? descripcion)
    {
        if (Fase is FaseOportunidad.Ganada or FaseOportunidad.Perdida)
            return Result<bool>.Failure(Error.Validation("Oportunidad.EstadoTerminal", "No se puede editar una oportunidad Ganada o Perdida"));
        if (importeEstimado <= 0)
            return Result<bool>.Failure(OportunidadErrors.ImporteInvalido);
        Titulo = titulo.Trim();
        ImporteEstimado = importeEstimado;
        FechaCierreEstimada = fechaCierreEstimada;
        Descripcion = descripcion;
        MarkUpdated();
        return Result<bool>.Success(true);
    }

    public void AvanzarFase(FaseOportunidad nueva, int? probabilidadPct = null)
    {
        Fase = nueva;
        if (probabilidadPct.HasValue) ProbabilidadPct = Math.Clamp(probabilidadPct.Value, 0, 100);
        else if (nueva == FaseOportunidad.Ganada) ProbabilidadPct = 100;
        else if (nueva == FaseOportunidad.Perdida) ProbabilidadPct = 0;
        MarkUpdated();
    }
}
