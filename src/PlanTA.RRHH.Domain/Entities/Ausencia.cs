using PlanTA.RRHH.Domain.Enums;
using PlanTA.RRHH.Domain.Errors;
using PlanTA.SharedKernel;

namespace PlanTA.RRHH.Domain.Entities;

public class Ausencia : SoftDeletableEntity<AusenciaId>
{
    private Ausencia() { }

    public EmpleadoId EmpleadoId { get; private set; } = default!;
    public TipoAusencia Tipo { get; private set; }
    public EstadoAusencia Estado { get; private set; } = EstadoAusencia.Solicitada;
    public DateTimeOffset FechaInicio { get; private set; }
    public DateTimeOffset FechaFin { get; private set; }
    public string? Motivo { get; private set; }
    public Guid? AprobadoPorUserId { get; private set; }
    public DateTimeOffset? FechaAprobacion { get; private set; }
    public Guid EmpresaId { get; private set; }

    public int DiasTotales => (int)Math.Ceiling((FechaFin - FechaInicio).TotalDays) + 1;

    public static Result<Ausencia> Crear(
        EmpleadoId empleadoId, TipoAusencia tipo, DateTimeOffset fechaInicio, DateTimeOffset fechaFin,
        Guid empresaId, string? motivo = null)
    {
        if (fechaFin < fechaInicio)
            return Result<Ausencia>.Failure(AusenciaErrors.FechasInvalidas);

        return Result<Ausencia>.Success(new Ausencia
        {
            Id = new AusenciaId(Guid.NewGuid()),
            EmpleadoId = empleadoId,
            Tipo = tipo,
            FechaInicio = fechaInicio,
            FechaFin = fechaFin,
            Motivo = motivo,
            EmpresaId = empresaId
        });
    }

    public void Aprobar(Guid userId)
    {
        Estado = EstadoAusencia.Aprobada;
        AprobadoPorUserId = userId;
        FechaAprobacion = DateTimeOffset.UtcNow;
        MarkUpdated();
    }

    public void Rechazar(Guid userId)
    {
        Estado = EstadoAusencia.Rechazada;
        AprobadoPorUserId = userId;
        FechaAprobacion = DateTimeOffset.UtcNow;
        MarkUpdated();
    }

    public Result<bool> Editar(TipoAusencia tipo, DateTimeOffset fechaInicio, DateTimeOffset fechaFin, string? motivo)
    {
        if (Estado is EstadoAusencia.Aprobada or EstadoAusencia.Rechazada or EstadoAusencia.Finalizada)
            return Result<bool>.Failure(Error.Validation("Ausencia.EstadoTerminal", "No se puede editar una ausencia ya procesada"));
        if (fechaFin < fechaInicio)
            return Result<bool>.Failure(AusenciaErrors.FechasInvalidas);
        Tipo = tipo;
        FechaInicio = fechaInicio;
        FechaFin = fechaFin;
        Motivo = motivo;
        MarkUpdated();
        return Result<bool>.Success(true);
    }
}
