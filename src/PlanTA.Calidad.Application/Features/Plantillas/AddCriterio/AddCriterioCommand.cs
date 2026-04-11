using PlanTA.SharedKernel.CQRS;

namespace PlanTA.Calidad.Application.Features.Plantillas.AddCriterio;

public record AddCriterioCommand(
    Guid PlantillaId,
    string Nombre,
    string TipoMedida,
    bool EsObligatorio,
    string? Descripcion = null,
    decimal? ValorMinimo = null,
    decimal? ValorMaximo = null,
    string? UnidadMedida = null) : ICommand<Guid>;
