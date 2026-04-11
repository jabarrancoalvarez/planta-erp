using PlanTA.SharedKernel;

namespace PlanTA.Produccion.Domain.Entities;

public class ParteProduccion : BaseEntity<ParteProduccionId>
{
    private ParteProduccion() { }

    public OrdenFabricacionId OrdenFabricacionId { get; private set; } = default!;
    public DateTimeOffset FechaRegistro { get; private set; }
    public decimal UnidadesBuenas { get; private set; }
    public decimal UnidadesDefectuosas { get; private set; }
    public decimal Merma { get; private set; }
    public Guid? LoteGeneradoId { get; private set; }
    public string? Observaciones { get; private set; }

    internal static ParteProduccion Crear(
        OrdenFabricacionId ordenFabricacionId,
        decimal unidadesBuenas,
        decimal unidadesDefectuosas,
        decimal merma,
        Guid? loteGeneradoId = null,
        string? observaciones = null)
    {
        return new ParteProduccion
        {
            Id = new ParteProduccionId(Guid.NewGuid()),
            OrdenFabricacionId = ordenFabricacionId,
            FechaRegistro = DateTimeOffset.UtcNow,
            UnidadesBuenas = unidadesBuenas,
            UnidadesDefectuosas = unidadesDefectuosas,
            Merma = merma,
            LoteGeneradoId = loteGeneradoId,
            Observaciones = observaciones
        };
    }
}
