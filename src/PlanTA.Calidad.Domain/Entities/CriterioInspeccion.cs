using PlanTA.SharedKernel;

namespace PlanTA.Calidad.Domain.Entities;

public class CriterioInspeccion : BaseEntity<CriterioInspeccionId>
{
    private CriterioInspeccion() { }

    public PlantillaInspeccionId PlantillaInspeccionId { get; private set; } = default!;
    public string Nombre { get; private set; } = string.Empty;
    public string? Descripcion { get; private set; }
    public string TipoMedida { get; private set; } = string.Empty;
    public decimal? ValorMinimo { get; private set; }
    public decimal? ValorMaximo { get; private set; }
    public string? UnidadMedida { get; private set; }
    public bool EsObligatorio { get; private set; }
    public int Orden { get; private set; }

    public static CriterioInspeccion Crear(
        PlantillaInspeccionId plantillaId,
        string nombre,
        string tipoMedida,
        bool esObligatorio,
        int orden,
        string? descripcion = null,
        decimal? valorMinimo = null,
        decimal? valorMaximo = null,
        string? unidadMedida = null)
    {
        return new CriterioInspeccion
        {
            Id = new CriterioInspeccionId(Guid.NewGuid()),
            PlantillaInspeccionId = plantillaId,
            Nombre = nombre,
            TipoMedida = tipoMedida,
            EsObligatorio = esObligatorio,
            Orden = orden,
            Descripcion = descripcion,
            ValorMinimo = valorMinimo,
            ValorMaximo = valorMaximo,
            UnidadMedida = unidadMedida
        };
    }
}
