using PlanTA.Calidad.Domain.Enums;
using PlanTA.Calidad.Domain.Errors;
using PlanTA.Calidad.Domain.Events;
using PlanTA.SharedKernel;

namespace PlanTA.Calidad.Domain.Entities;

public class Inspeccion : AggregateRoot<InspeccionId>
{
    private readonly List<ResultadoCriterio> _resultados = [];
    private Inspeccion() { }

    public PlantillaInspeccionId PlantillaInspeccionId { get; private set; } = default!;
    public OrigenInspeccion OrigenInspeccion { get; private set; }
    public Guid ReferenciaOrigenId { get; private set; }
    public Guid? LoteId { get; private set; }
    public DateTimeOffset FechaInspeccion { get; private set; }
    public string? InspectorUserId { get; private set; }
    public ResultadoInspeccion? ResultadoInspeccion { get; private set; }
    public string? Observaciones { get; private set; }
    public Guid EmpresaId { get; private set; }

    public IReadOnlyList<ResultadoCriterio> Resultados => _resultados.AsReadOnly();

    public static Inspeccion Crear(
        PlantillaInspeccionId plantillaId,
        OrigenInspeccion origen,
        Guid referenciaOrigenId,
        Guid empresaId,
        Guid? loteId = null,
        string? inspectorUserId = null,
        string? observaciones = null)
    {
        return new Inspeccion
        {
            Id = new InspeccionId(Guid.NewGuid()),
            PlantillaInspeccionId = plantillaId,
            OrigenInspeccion = origen,
            ReferenciaOrigenId = referenciaOrigenId,
            LoteId = loteId,
            FechaInspeccion = DateTimeOffset.UtcNow,
            InspectorUserId = inspectorUserId,
            Observaciones = observaciones,
            EmpresaId = empresaId
        };
    }

    public void AgregarResultadoVacio(CriterioInspeccion criterio)
    {
        var resultado = ResultadoCriterio.Crear(Id, criterio.Id, criterio.Nombre, criterio.EsObligatorio);
        _resultados.Add(resultado);
    }

    public Result<bool> RegistrarResultado(
        CriterioInspeccionId criterioId, string? valorMedido, bool cumple, string? observaciones)
    {
        if (ResultadoInspeccion is not null)
            return Result<bool>.Failure(InspeccionErrors.YaCompletada(Id.Value));

        var resultado = _resultados.FirstOrDefault(r => r.CriterioInspeccionId == criterioId);
        if (resultado is null)
            return Result<bool>.Failure(InspeccionErrors.ResultadoCriterioNotFound(criterioId.Value));

        resultado.Registrar(valorMedido, cumple, observaciones);
        MarkUpdated();
        return Result<bool>.Success(true);
    }

    public Result<ResultadoInspeccion> Completar()
    {
        if (ResultadoInspeccion is not null)
            return Result<ResultadoInspeccion>.Failure(InspeccionErrors.YaCompletada(Id.Value));

        // Check all obligatorio results have been registered (have UpdatedAt set)
        var obligatoriosSinResultado = _resultados
            .Where(r => r.EsObligatorio && r.UpdatedAt is null)
            .ToList();

        if (obligatoriosSinResultado.Count > 0)
            return Result<ResultadoInspeccion>.Failure(InspeccionErrors.ResultadosPendientes(Id.Value));

        // Evaluate result
        var obligatoriosFallidos = _resultados.Where(r => r.EsObligatorio && !r.Cumple).ToList();
        var noObligatoriosFallidos = _resultados.Where(r => !r.EsObligatorio && r.UpdatedAt is not null && !r.Cumple).ToList();

        ResultadoInspeccion resultado;
        if (obligatoriosFallidos.Count > 0)
        {
            resultado = Enums.ResultadoInspeccion.Rechazada;
        }
        else if (noObligatoriosFallidos.Count > 0)
        {
            resultado = Enums.ResultadoInspeccion.AprobadaConObservaciones;
        }
        else
        {
            resultado = Enums.ResultadoInspeccion.Aprobada;
        }

        ResultadoInspeccion = resultado;
        MarkUpdated();

        AddDomainEvent(new InspeccionCompletadaEvent(Id, resultado));

        if (resultado == Enums.ResultadoInspeccion.Rechazada)
        {
            AddDomainEvent(new InspeccionRechazadaEvent(Id, LoteId));
        }

        return Result<ResultadoInspeccion>.Success(resultado);
    }
}
