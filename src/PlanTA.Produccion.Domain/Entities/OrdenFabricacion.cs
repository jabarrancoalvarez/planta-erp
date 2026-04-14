using PlanTA.Produccion.Domain.Enums;
using PlanTA.Produccion.Domain.Errors;
using PlanTA.Produccion.Domain.Events;
using PlanTA.Produccion.Domain.ValueObjects;
using PlanTA.SharedKernel;

namespace PlanTA.Produccion.Domain.Entities;

public class OrdenFabricacion : SoftDeletableEntity<OrdenFabricacionId>
{
    private readonly List<LineaConsumoOF> _lineasConsumo = [];
    private readonly List<ParteProduccion> _partesProduccion = [];
    private OrdenFabricacion() { }

    public CodigoOF CodigoOF { get; private set; } = default!;
    public Guid ProductoId { get; private set; }
    public ListaMaterialesId ListaMaterialesId { get; private set; } = default!;
    public RutaProduccionId? RutaProduccionId { get; private set; }
    public CantidadPlanificada CantidadPlanificada { get; private set; } = default!;
    public EstadoOF EstadoOF { get; private set; } = EstadoOF.Planificada;
    public DateTimeOffset? FechaInicio { get; private set; }
    public DateTimeOffset? FechaFin { get; private set; }
    public int Prioridad { get; private set; }
    public string? Observaciones { get; private set; }
    public Guid EmpresaId { get; private set; }

    public IReadOnlyList<LineaConsumoOF> LineasConsumo => _lineasConsumo.AsReadOnly();
    public IReadOnlyList<ParteProduccion> PartesProduccion => _partesProduccion.AsReadOnly();

    public static OrdenFabricacion Crear(
        string codigoOF,
        Guid productoId,
        ListaMaterialesId listaMaterialesId,
        decimal cantidadPlanificada,
        string unidadMedida,
        Guid empresaId,
        RutaProduccionId? rutaProduccionId = null,
        int prioridad = 0,
        string? observaciones = null)
    {
        var of = new OrdenFabricacion
        {
            Id = new OrdenFabricacionId(Guid.NewGuid()),
            CodigoOF = CodigoOF.Create(codigoOF),
            ProductoId = productoId,
            ListaMaterialesId = listaMaterialesId,
            RutaProduccionId = rutaProduccionId,
            CantidadPlanificada = new CantidadPlanificada(cantidadPlanificada, unidadMedida),
            Prioridad = prioridad,
            Observaciones = observaciones,
            EmpresaId = empresaId
        };

        of.AddDomainEvent(new OFCreadaEvent(of.Id, productoId, cantidadPlanificada));
        return of;
    }

    public Result<bool> Editar(int prioridad, string? observaciones)
    {
        if (EstadoOF is EstadoOF.Completada or EstadoOF.Cancelada)
            return Result<bool>.Failure(
                OrdenFabricacionErrors.TransicionInvalida(EstadoOF.ToString(), "Editar"));

        Prioridad = prioridad;
        Observaciones = observaciones;
        MarkUpdated();
        return Result<bool>.Success(true);
    }

    public Result<bool> Preparar()
    {
        if (EstadoOF != EstadoOF.Planificada)
            return Result<bool>.Failure(
                OrdenFabricacionErrors.TransicionInvalida(EstadoOF.ToString(), nameof(EstadoOF.EnPreparacion)));

        EstadoOF = EstadoOF.EnPreparacion;
        MarkUpdated();
        return Result<bool>.Success(true);
    }

    public Result<bool> Iniciar()
    {
        if (EstadoOF != EstadoOF.EnPreparacion)
            return Result<bool>.Failure(
                OrdenFabricacionErrors.TransicionInvalida(EstadoOF.ToString(), nameof(EstadoOF.EnCurso)));

        EstadoOF = EstadoOF.EnCurso;
        FechaInicio = DateTimeOffset.UtcNow;
        MarkUpdated();
        AddDomainEvent(new OFIniciadaEvent(Id));
        return Result<bool>.Success(true);
    }

    public Result<bool> Pausar()
    {
        if (EstadoOF != EstadoOF.EnCurso)
            return Result<bool>.Failure(
                OrdenFabricacionErrors.TransicionInvalida(EstadoOF.ToString(), nameof(EstadoOF.Pausada)));

        EstadoOF = EstadoOF.Pausada;
        MarkUpdated();
        return Result<bool>.Success(true);
    }

    public Result<bool> Reanudar()
    {
        if (EstadoOF != EstadoOF.Pausada)
            return Result<bool>.Failure(
                OrdenFabricacionErrors.TransicionInvalida(EstadoOF.ToString(), nameof(EstadoOF.EnCurso)));

        EstadoOF = EstadoOF.EnCurso;
        MarkUpdated();
        return Result<bool>.Success(true);
    }

    public Result<bool> Completar()
    {
        if (EstadoOF != EstadoOF.EnCurso)
            return Result<bool>.Failure(
                OrdenFabricacionErrors.TransicionInvalida(EstadoOF.ToString(), nameof(EstadoOF.Completada)));

        EstadoOF = EstadoOF.Completada;
        FechaFin = DateTimeOffset.UtcNow;
        MarkUpdated();

        var totalBuenas = _partesProduccion.Sum(p => p.UnidadesBuenas);
        var totalDefectuosas = _partesProduccion.Sum(p => p.UnidadesDefectuosas);
        AddDomainEvent(new OFCompletadaEvent(Id, totalBuenas, totalDefectuosas));
        return Result<bool>.Success(true);
    }

    public Result<bool> Cancelar(string motivo)
    {
        if (EstadoOF is EstadoOF.Completada or EstadoOF.Cancelada)
            return Result<bool>.Failure(
                OrdenFabricacionErrors.TransicionInvalida(EstadoOF.ToString(), nameof(EstadoOF.Cancelada)));

        EstadoOF = EstadoOF.Cancelada;
        FechaFin = DateTimeOffset.UtcNow;
        Observaciones = motivo;
        MarkUpdated();
        AddDomainEvent(new OFCanceladaEvent(Id, motivo));
        return Result<bool>.Success(true);
    }

    public Result<LineaConsumoOF> RegistrarConsumo(Guid productoId, decimal cantidad, Guid? loteId = null)
    {
        if (EstadoOF != EstadoOF.EnCurso)
            return Result<LineaConsumoOF>.Failure(OrdenFabricacionErrors.NoEnCurso(Id.Value));

        var linea = LineaConsumoOF.Crear(Id, productoId, cantidad, loteId);
        _lineasConsumo.Add(linea);
        MarkUpdated();
        AddDomainEvent(new MaterialConsumidoEvent(Id, productoId, loteId, cantidad));
        return Result<LineaConsumoOF>.Success(linea);
    }

    public Result<ParteProduccion> RegistrarProduccion(
        decimal unidadesBuenas,
        decimal unidadesDefectuosas,
        decimal merma,
        Guid? loteGeneradoId = null,
        string? observaciones = null)
    {
        if (EstadoOF != EstadoOF.EnCurso)
            return Result<ParteProduccion>.Failure(OrdenFabricacionErrors.NoEnCurso(Id.Value));

        var parte = ParteProduccion.Crear(Id, unidadesBuenas, unidadesDefectuosas, merma, loteGeneradoId, observaciones);
        _partesProduccion.Add(parte);
        MarkUpdated();
        AddDomainEvent(new ProduccionRegistradaEvent(Id, unidadesBuenas, loteGeneradoId));
        return Result<ParteProduccion>.Success(parte);
    }
}
