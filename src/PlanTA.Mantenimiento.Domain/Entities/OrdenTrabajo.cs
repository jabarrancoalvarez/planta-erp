using PlanTA.Mantenimiento.Domain.Enums;
using PlanTA.Mantenimiento.Domain.Events;
using PlanTA.Mantenimiento.Domain.Errors;
using PlanTA.SharedKernel;

namespace PlanTA.Mantenimiento.Domain.Entities;

public class OrdenTrabajo : SoftDeletableEntity<OrdenTrabajoId>
{
    private readonly List<TareaOT> _tareas = [];
    private readonly List<RepuestoOT> _repuestos = [];

    private OrdenTrabajo() { }

    public string Codigo { get; private set; } = string.Empty;
    public string Titulo { get; private set; } = string.Empty;
    public string? Descripcion { get; private set; }
    public TipoMantenimiento Tipo { get; private set; }
    public EstadoOT Estado { get; private set; } = EstadoOT.Borrador;
    public PrioridadOT Prioridad { get; private set; } = PrioridadOT.Media;
    public Guid ActivoId { get; private set; }
    public Guid? PlanMantenimientoId { get; private set; }
    public Guid? IncidenciaId { get; private set; }
    public Guid? AsignadoAUserId { get; private set; }
    public DateTimeOffset? FechaPlanificada { get; private set; }
    public DateTimeOffset? FechaInicio { get; private set; }
    public DateTimeOffset? FechaFin { get; private set; }
    public decimal HorasEstimadas { get; private set; }
    public decimal HorasReales { get; private set; }
    public decimal CosteManoObra { get; private set; }
    public decimal CosteRepuestos { get; private set; }
    public string? NotasCierre { get; private set; }
    public Guid EmpresaId { get; private set; }

    public IReadOnlyCollection<TareaOT> Tareas => _tareas.AsReadOnly();
    public IReadOnlyCollection<RepuestoOT> Repuestos => _repuestos.AsReadOnly();

    public static OrdenTrabajo Crear(
        string codigo, string titulo, TipoMantenimiento tipo, Guid activoId,
        Guid empresaId, string? descripcion = null, PrioridadOT prioridad = PrioridadOT.Media,
        DateTimeOffset? fechaPlanificada = null, decimal horasEstimadas = 0,
        Guid? planId = null, Guid? incidenciaId = null)
    {
        var ot = new OrdenTrabajo
        {
            Id = new OrdenTrabajoId(Guid.NewGuid()),
            Codigo = codigo.Trim().ToUpperInvariant(),
            Titulo = titulo.Trim(),
            Descripcion = descripcion,
            Tipo = tipo,
            Prioridad = prioridad,
            ActivoId = activoId,
            PlanMantenimientoId = planId,
            IncidenciaId = incidenciaId,
            FechaPlanificada = fechaPlanificada,
            HorasEstimadas = horasEstimadas,
            Estado = fechaPlanificada.HasValue ? EstadoOT.Planificada : EstadoOT.Borrador,
            EmpresaId = empresaId
        };
        ot.AddDomainEvent(new OrdenTrabajoCreadaEvent(ot.Id, activoId, (int)tipo));
        return ot;
    }

    public void Asignar(Guid userId)
    {
        AsignadoAUserId = userId;
        MarkUpdated();
    }

    public Result<bool> Iniciar()
    {
        if (Estado is EstadoOT.Completada or EstadoOT.Cancelada)
            return Result<bool>.Failure(OrdenTrabajoErrors.EstadoInvalido("No se puede iniciar una OT completada o cancelada"));

        Estado = EstadoOT.EnEjecucion;
        FechaInicio = DateTimeOffset.UtcNow;
        MarkUpdated();
        return Result<bool>.Success(true);
    }

    public Result<bool> Completar(decimal horasReales, decimal costeManoObra, string? notasCierre)
    {
        if (Estado == EstadoOT.Completada)
            return Result<bool>.Failure(OrdenTrabajoErrors.YaCompletada(Id.Value));

        Estado = EstadoOT.Completada;
        FechaFin = DateTimeOffset.UtcNow;
        HorasReales = horasReales;
        CosteManoObra = costeManoObra;
        NotasCierre = notasCierre;
        MarkUpdated();
        AddDomainEvent(new OrdenTrabajoCompletadaEvent(Id, ActivoId, horasReales));
        return Result<bool>.Success(true);
    }

    public void Cancelar(string motivo)
    {
        Estado = EstadoOT.Cancelada;
        NotasCierre = motivo;
        MarkUpdated();
    }

    public void AgregarTarea(TareaOT tarea) => _tareas.Add(tarea);

    public void AgregarRepuesto(RepuestoOT repuesto)
    {
        _repuestos.Add(repuesto);
        CosteRepuestos += repuesto.CosteTotal;
    }
}
