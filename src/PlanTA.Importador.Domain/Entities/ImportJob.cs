using PlanTA.Importador.Domain.Enums;
using PlanTA.SharedKernel;

namespace PlanTA.Importador.Domain.Entities;

public class ImportJob : BaseEntity<ImportJobId>
{
    private ImportJob() { }

    public TipoImportacion Tipo { get; private set; }
    public FormatoArchivo Formato { get; private set; }
    public EstadoImportJob Estado { get; private set; } = EstadoImportJob.Pendiente;
    public string NombreArchivo { get; private set; } = string.Empty;
    public int FilasTotales { get; private set; }
    public int FilasValidas { get; private set; }
    public int FilasConError { get; private set; }
    public int FilasImportadas { get; private set; }
    public string? ResumenErrores { get; private set; }
    public Guid CreadoPorUserId { get; private set; }
    public DateTimeOffset? IniciadoEn { get; private set; }
    public DateTimeOffset? FinalizadoEn { get; private set; }
    public Guid EmpresaId { get; private set; }

    public static ImportJob Crear(
        TipoImportacion tipo, FormatoArchivo formato, string nombreArchivo,
        Guid userId, Guid empresaId)
        => new()
        {
            Id = new ImportJobId(Guid.NewGuid()),
            Tipo = tipo,
            Formato = formato,
            NombreArchivo = nombreArchivo,
            CreadoPorUserId = userId,
            EmpresaId = empresaId
        };

    public void RegistrarValidacion(int total, int validas, int errores, string? resumen = null)
    {
        FilasTotales = total;
        FilasValidas = validas;
        FilasConError = errores;
        ResumenErrores = resumen;
        Estado = errores == 0 ? EstadoImportJob.ListaParaImportar : EstadoImportJob.ListaParaImportar;
        MarkUpdated();
    }

    public void IniciarImportacion()
    {
        Estado = EstadoImportJob.Importando;
        IniciadoEn = DateTimeOffset.UtcNow;
        MarkUpdated();
    }

    public void Completar(int importadas, bool conErrores)
    {
        FilasImportadas = importadas;
        Estado = conErrores ? EstadoImportJob.CompletadaConErrores : EstadoImportJob.Completada;
        FinalizadoEn = DateTimeOffset.UtcNow;
        MarkUpdated();
    }

    public void Fallar(string motivo)
    {
        Estado = EstadoImportJob.Fallida;
        ResumenErrores = motivo;
        FinalizadoEn = DateTimeOffset.UtcNow;
        MarkUpdated();
    }

    public void Cancelar()
    {
        Estado = EstadoImportJob.Cancelada;
        FinalizadoEn = DateTimeOffset.UtcNow;
        MarkUpdated();
    }
}
