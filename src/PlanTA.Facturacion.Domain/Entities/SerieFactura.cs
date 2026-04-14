using PlanTA.SharedKernel;

namespace PlanTA.Facturacion.Domain.Entities;

public class SerieFactura : SoftDeletableEntity<SerieFacturaId>
{
    private SerieFactura() { }

    public string Codigo { get; private set; } = string.Empty;
    public string Descripcion { get; private set; } = string.Empty;
    public int Ejercicio { get; private set; }
    public int UltimoNumero { get; private set; }
    public Guid EmpresaId { get; private set; }

    public static SerieFactura Crear(string codigo, string descripcion, int ejercicio, Guid empresaId)
        => new()
        {
            Id = new SerieFacturaId(Guid.NewGuid()),
            Codigo = codigo.Trim().ToUpperInvariant(),
            Descripcion = descripcion,
            Ejercicio = ejercicio,
            EmpresaId = empresaId
        };

    public int SiguienteNumero()
    {
        UltimoNumero++;
        MarkUpdated();
        return UltimoNumero;
    }
}
