using PlanTA.Calidad.Domain.Enums;
using PlanTA.Calidad.Domain.Errors;
using PlanTA.Calidad.Domain.Events;
using PlanTA.SharedKernel;

namespace PlanTA.Calidad.Domain.Entities;

public class NoConformidad : SoftDeletableEntity<NoConformidadId>
{
    private readonly List<AccionCorrectiva> _acciones = [];
    private NoConformidad() { }

    public string Codigo { get; private set; } = string.Empty;
    public InspeccionId? InspeccionId { get; private set; }
    public OrigenInspeccion OrigenInspeccion { get; private set; }
    public Guid ReferenciaOrigenId { get; private set; }
    public string Descripcion { get; private set; } = string.Empty;
    public SeveridadNC SeveridadNC { get; private set; }
    public EstadoNoConformidad EstadoNoConformidad { get; private set; } = EstadoNoConformidad.Abierta;
    public string? CausaRaiz { get; private set; }
    public DateTimeOffset FechaDeteccion { get; private set; }
    public DateTimeOffset? FechaCierre { get; private set; }
    public string? ResponsableUserId { get; private set; }
    public Guid EmpresaId { get; private set; }

    public IReadOnlyList<AccionCorrectiva> Acciones => _acciones.AsReadOnly();

    public static NoConformidad Crear(
        string codigo,
        OrigenInspeccion origen,
        Guid referenciaOrigenId,
        string descripcion,
        SeveridadNC severidad,
        Guid empresaId,
        InspeccionId? inspeccionId = null,
        string? responsableUserId = null)
    {
        var nc = new NoConformidad
        {
            Id = new NoConformidadId(Guid.NewGuid()),
            Codigo = codigo.Trim().ToUpperInvariant(),
            OrigenInspeccion = origen,
            ReferenciaOrigenId = referenciaOrigenId,
            Descripcion = descripcion,
            SeveridadNC = severidad,
            EmpresaId = empresaId,
            InspeccionId = inspeccionId,
            ResponsableUserId = responsableUserId,
            FechaDeteccion = DateTimeOffset.UtcNow
        };

        nc.AddDomainEvent(new NoConformidadAbiertaEvent(nc.Id, origen));
        return nc;
    }

    public Result<bool> Editar(string descripcion, SeveridadNC severidad, string? responsableUserId)
    {
        if (EstadoNoConformidad is EstadoNoConformidad.Cerrada)
            return Result<bool>.Failure(
                NoConformidadErrors.TransicionInvalida(EstadoNoConformidad.ToString(), "Editar"));

        Descripcion = descripcion;
        SeveridadNC = severidad;
        ResponsableUserId = responsableUserId;
        MarkUpdated();
        return Result<bool>.Success(true);
    }

    public Result<bool> Investigar()
    {
        if (EstadoNoConformidad != EstadoNoConformidad.Abierta)
            return Result<bool>.Failure(
                NoConformidadErrors.TransicionInvalida(
                    EstadoNoConformidad.ToString(), nameof(EstadoNoConformidad.EnInvestigacion)));

        EstadoNoConformidad = EstadoNoConformidad.EnInvestigacion;
        MarkUpdated();
        return Result<bool>.Success(true);
    }

    public Result<bool> DefinirAccion(string? causaRaiz = null)
    {
        if (EstadoNoConformidad != EstadoNoConformidad.EnInvestigacion)
            return Result<bool>.Failure(
                NoConformidadErrors.TransicionInvalida(
                    EstadoNoConformidad.ToString(), nameof(EstadoNoConformidad.AccionDefinida)));

        EstadoNoConformidad = EstadoNoConformidad.AccionDefinida;
        if (causaRaiz is not null) CausaRaiz = causaRaiz;
        MarkUpdated();
        return Result<bool>.Success(true);
    }

    public Result<bool> Resolver()
    {
        if (EstadoNoConformidad != EstadoNoConformidad.AccionDefinida)
            return Result<bool>.Failure(
                NoConformidadErrors.TransicionInvalida(
                    EstadoNoConformidad.ToString(), nameof(EstadoNoConformidad.Resuelta)));

        EstadoNoConformidad = EstadoNoConformidad.Resuelta;
        MarkUpdated();
        AddDomainEvent(new NoConformidadResueltaEvent(Id));
        return Result<bool>.Success(true);
    }

    public Result<bool> Cerrar()
    {
        if (EstadoNoConformidad != EstadoNoConformidad.Resuelta)
            return Result<bool>.Failure(
                NoConformidadErrors.TransicionInvalida(
                    EstadoNoConformidad.ToString(), nameof(EstadoNoConformidad.Cerrada)));

        EstadoNoConformidad = EstadoNoConformidad.Cerrada;
        FechaCierre = DateTimeOffset.UtcNow;
        MarkUpdated();
        return Result<bool>.Success(true);
    }

    public AccionCorrectiva AgregarAccion(
        string descripcion, string? responsableUserId = null, DateTimeOffset? fechaLimite = null)
    {
        var accion = AccionCorrectiva.Crear(Id, descripcion, responsableUserId, fechaLimite);
        _acciones.Add(accion);
        MarkUpdated();
        return accion;
    }
}
