using PlanTA.SharedKernel;

namespace PlanTA.Mantenimiento.Domain.Entities;

public class RepuestoOT : BaseEntity<RepuestoOTId>
{
    private RepuestoOT() { }

    public OrdenTrabajoId OrdenTrabajoId { get; private set; } = default!;
    public Guid ProductoId { get; private set; }
    public string Descripcion { get; private set; } = string.Empty;
    public decimal Cantidad { get; private set; }
    public decimal CosteUnitario { get; private set; }
    public decimal CosteTotal => Cantidad * CosteUnitario;

    public static RepuestoOT Crear(OrdenTrabajoId otId, Guid productoId, string descripcion, decimal cantidad, decimal costeUnitario)
        => new()
        {
            Id = new RepuestoOTId(Guid.NewGuid()),
            OrdenTrabajoId = otId,
            ProductoId = productoId,
            Descripcion = descripcion,
            Cantidad = cantidad,
            CosteUnitario = costeUnitario
        };
}
