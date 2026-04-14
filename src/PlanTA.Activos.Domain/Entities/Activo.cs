using PlanTA.Activos.Domain.Enums;
using PlanTA.Activos.Domain.Events;
using PlanTA.SharedKernel;

namespace PlanTA.Activos.Domain.Entities;

public class Activo : SoftDeletableEntity<ActivoId>
{
    private readonly List<DocumentoActivo> _documentos = [];
    private readonly List<LecturaActivo> _lecturas = [];

    private Activo() { }

    public string Codigo { get; private set; } = string.Empty;
    public string Nombre { get; private set; } = string.Empty;
    public string? Descripcion { get; private set; }
    public TipoActivo Tipo { get; private set; }
    public CriticidadActivo Criticidad { get; private set; }
    public EstadoActivo Estado { get; private set; } = EstadoActivo.Operativo;
    public ActivoId? ActivoPadreId { get; private set; }
    public string? Ubicacion { get; private set; }
    public string? Fabricante { get; private set; }
    public string? Modelo { get; private set; }
    public string? NumeroSerie { get; private set; }
    public DateTimeOffset? FechaAdquisicion { get; private set; }
    public decimal CosteAdquisicion { get; private set; }
    public decimal HorasUso { get; private set; }
    public Guid EmpresaId { get; private set; }

    public IReadOnlyCollection<DocumentoActivo> Documentos => _documentos.AsReadOnly();
    public IReadOnlyCollection<LecturaActivo> Lecturas => _lecturas.AsReadOnly();

    public static Activo Crear(
        string codigo, string nombre, TipoActivo tipo, CriticidadActivo criticidad,
        Guid empresaId, string? descripcion = null, ActivoId? activoPadreId = null,
        string? ubicacion = null, string? fabricante = null, string? modelo = null,
        string? numeroSerie = null, DateTimeOffset? fechaAdquisicion = null, decimal costeAdquisicion = 0)
    {
        var activo = new Activo
        {
            Id = new ActivoId(Guid.NewGuid()),
            Codigo = codigo.Trim().ToUpperInvariant(),
            Nombre = nombre.Trim(),
            Descripcion = descripcion,
            Tipo = tipo,
            Criticidad = criticidad,
            ActivoPadreId = activoPadreId,
            Ubicacion = ubicacion,
            Fabricante = fabricante,
            Modelo = modelo,
            NumeroSerie = numeroSerie,
            FechaAdquisicion = fechaAdquisicion,
            CosteAdquisicion = costeAdquisicion,
            EmpresaId = empresaId
        };

        activo.AddDomainEvent(new ActivoCreadoEvent(activo.Id, activo.Codigo));
        return activo;
    }

    public void Actualizar(string nombre, string? descripcion, CriticidadActivo criticidad,
        string? ubicacion, string? fabricante, string? modelo)
    {
        Nombre = nombre.Trim();
        Descripcion = descripcion;
        Criticidad = criticidad;
        Ubicacion = ubicacion;
        Fabricante = fabricante;
        Modelo = modelo;
        MarkUpdated();
    }

    public void CambiarEstado(EstadoActivo nuevoEstado)
    {
        if (Estado == nuevoEstado) return;
        var anterior = Estado;
        Estado = nuevoEstado;
        MarkUpdated();
        AddDomainEvent(new EstadoActivoCambiadoEvent(Id, (int)anterior, (int)nuevoEstado));
    }

    public void RegistrarHorasUso(decimal horas)
    {
        if (horas < 0) return;
        HorasUso += horas;
        MarkUpdated();
    }

    public void AdjuntarDocumento(DocumentoActivo documento) => _documentos.Add(documento);
    public void RegistrarLectura(LecturaActivo lectura) => _lecturas.Add(lectura);
}
