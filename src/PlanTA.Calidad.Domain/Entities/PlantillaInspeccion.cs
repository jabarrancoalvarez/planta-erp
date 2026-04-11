using PlanTA.Calidad.Domain.Enums;
using PlanTA.Calidad.Domain.Errors;
using PlanTA.SharedKernel;

namespace PlanTA.Calidad.Domain.Entities;

public class PlantillaInspeccion : SoftDeletableEntity<PlantillaInspeccionId>
{
    private readonly List<CriterioInspeccion> _criterios = [];
    private PlantillaInspeccion() { }

    public string Nombre { get; private set; } = string.Empty;
    public string? Descripcion { get; private set; }
    public OrigenInspeccion OrigenInspeccion { get; private set; }
    public new int Version { get; private set; } = 1;
    public bool Activa { get; private set; } = true;
    public Guid EmpresaId { get; private set; }

    public IReadOnlyList<CriterioInspeccion> Criterios => _criterios.AsReadOnly();

    public static PlantillaInspeccion Crear(
        string nombre,
        OrigenInspeccion origen,
        Guid empresaId,
        string? descripcion = null)
    {
        return new PlantillaInspeccion
        {
            Id = new PlantillaInspeccionId(Guid.NewGuid()),
            Nombre = nombre,
            OrigenInspeccion = origen,
            EmpresaId = empresaId,
            Descripcion = descripcion
        };
    }

    public CriterioInspeccion AgregarCriterio(
        string nombre,
        string tipoMedida,
        bool esObligatorio,
        string? descripcion = null,
        decimal? valorMinimo = null,
        decimal? valorMaximo = null,
        string? unidadMedida = null)
    {
        var orden = _criterios.Count + 1;
        var criterio = CriterioInspeccion.Crear(
            Id, nombre, tipoMedida, esObligatorio, orden,
            descripcion, valorMinimo, valorMaximo, unidadMedida);
        _criterios.Add(criterio);
        MarkUpdated();
        return criterio;
    }

    public Result<bool> RemoverCriterio(CriterioInspeccionId criterioId)
    {
        var criterio = _criterios.FirstOrDefault(c => c.Id == criterioId);
        if (criterio is null)
            return Result<bool>.Failure(PlantillaErrors.CriterioNotFound(criterioId.Value));

        _criterios.Remove(criterio);
        MarkUpdated();
        return Result<bool>.Success(true);
    }

    public void Desactivar()
    {
        Activa = false;
        MarkUpdated();
    }
}
