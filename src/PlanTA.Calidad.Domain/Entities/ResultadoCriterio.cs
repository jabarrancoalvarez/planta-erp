using PlanTA.SharedKernel;

namespace PlanTA.Calidad.Domain.Entities;

public class ResultadoCriterio : BaseEntity<ResultadoCriterioId>
{
    private ResultadoCriterio() { }

    public InspeccionId InspeccionId { get; private set; } = default!;
    public CriterioInspeccionId CriterioInspeccionId { get; private set; } = default!;
    public string? ValorMedido { get; private set; }
    public bool Cumple { get; private set; }
    public string? Observaciones { get; private set; }

    /// <summary>Snapshot del criterio al momento de crear la inspeccion.</summary>
    public string NombreCriterio { get; private set; } = string.Empty;
    public bool EsObligatorio { get; private set; }

    public static ResultadoCriterio Crear(
        InspeccionId inspeccionId,
        CriterioInspeccionId criterioId,
        string nombreCriterio,
        bool esObligatorio)
    {
        return new ResultadoCriterio
        {
            Id = new ResultadoCriterioId(Guid.NewGuid()),
            InspeccionId = inspeccionId,
            CriterioInspeccionId = criterioId,
            NombreCriterio = nombreCriterio,
            EsObligatorio = esObligatorio
        };
    }

    public void Registrar(string? valorMedido, bool cumple, string? observaciones)
    {
        ValorMedido = valorMedido;
        Cumple = cumple;
        Observaciones = observaciones;
        MarkUpdated();
    }
}
