using PlanTA.Incidencias.Domain.Enums;
using PlanTA.Incidencias.Domain.Events;
using PlanTA.Incidencias.Domain.Errors;
using PlanTA.SharedKernel;

namespace PlanTA.Incidencias.Domain.Entities;

public class Incidencia : SoftDeletableEntity<IncidenciaId>
{
    private readonly List<FotoIncidencia> _fotos = [];
    private readonly List<ComentarioIncidencia> _comentarios = [];

    private Incidencia() { }

    public string Codigo { get; private set; } = string.Empty;
    public string Titulo { get; private set; } = string.Empty;
    public string Descripcion { get; private set; } = string.Empty;
    public TipoIncidencia Tipo { get; private set; }
    public SeveridadIncidencia Severidad { get; private set; }
    public EstadoIncidencia Estado { get; private set; } = EstadoIncidencia.Abierta;
    public Guid? ActivoId { get; private set; }
    public string? UbicacionTexto { get; private set; }
    public Guid ReportadoPorUserId { get; private set; }
    public Guid? AsignadoAUserId { get; private set; }
    public DateTimeOffset FechaApertura { get; private set; }
    public DateTimeOffset? FechaCierre { get; private set; }
    public Guid? OrdenTrabajoId { get; private set; }
    public string? CausaRaiz { get; private set; }
    public string? ResolucionNotas { get; private set; }
    public bool ParadaLinea { get; private set; }
    public Guid EmpresaId { get; private set; }

    public IReadOnlyCollection<FotoIncidencia> Fotos => _fotos.AsReadOnly();
    public IReadOnlyCollection<ComentarioIncidencia> Comentarios => _comentarios.AsReadOnly();

    public static Incidencia Crear(
        string codigo, string titulo, string descripcion, TipoIncidencia tipo,
        SeveridadIncidencia severidad, Guid reportadoPorUserId, Guid empresaId,
        Guid? activoId = null, string? ubicacionTexto = null, bool paradaLinea = false)
    {
        var inc = new Incidencia
        {
            Id = new IncidenciaId(Guid.NewGuid()),
            Codigo = codigo.Trim().ToUpperInvariant(),
            Titulo = titulo.Trim(),
            Descripcion = descripcion,
            Tipo = tipo,
            Severidad = severidad,
            ActivoId = activoId,
            UbicacionTexto = ubicacionTexto,
            ReportadoPorUserId = reportadoPorUserId,
            ParadaLinea = paradaLinea,
            FechaApertura = DateTimeOffset.UtcNow,
            EmpresaId = empresaId
        };
        inc.AddDomainEvent(new IncidenciaAbiertaEvent(inc.Id, activoId, (int)severidad, (int)tipo));
        return inc;
    }

    public void Asignar(Guid userId)
    {
        AsignadoAUserId = userId;
        if (Estado == EstadoIncidencia.Abierta) Estado = EstadoIncidencia.EnRevision;
        MarkUpdated();
    }

    public void VincularOT(Guid ordenTrabajoId)
    {
        OrdenTrabajoId = ordenTrabajoId;
        Estado = EstadoIncidencia.EnReparacion;
        MarkUpdated();
    }

    public Result<bool> Cerrar(string? causaRaiz, string? resolucionNotas)
    {
        if (Estado is EstadoIncidencia.Cerrada or EstadoIncidencia.Descartada)
            return Result<bool>.Failure(IncidenciaErrors.YaCerrada(Id.Value));

        CausaRaiz = causaRaiz;
        ResolucionNotas = resolucionNotas;
        Estado = EstadoIncidencia.Cerrada;
        FechaCierre = DateTimeOffset.UtcNow;
        MarkUpdated();
        AddDomainEvent(new IncidenciaCerradaEvent(Id, OrdenTrabajoId));
        return Result<bool>.Success(true);
    }

    public void AgregarFoto(FotoIncidencia foto) => _fotos.Add(foto);
    public void AgregarComentario(ComentarioIncidencia comentario) => _comentarios.Add(comentario);
}
