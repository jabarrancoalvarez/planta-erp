using PlanTA.Costes.Domain.Enums;
using PlanTA.Costes.Domain.Errors;
using PlanTA.SharedKernel;

namespace PlanTA.Costes.Domain.Entities;

public class ImputacionCoste : BaseEntity<ImputacionCosteId>
{
    private ImputacionCoste() { }

    public Guid? OrdenFabricacionId { get; private set; }
    public Guid? OrdenTrabajoId { get; private set; }
    public Guid? ProductoId { get; private set; }
    public TipoCoste Tipo { get; private set; }
    public OrigenImputacion Origen { get; private set; }
    public decimal Cantidad { get; private set; }
    public decimal PrecioUnitario { get; private set; }
    public decimal Importe { get; private set; }
    public string? Concepto { get; private set; }
    public DateTimeOffset Fecha { get; private set; }
    public Guid EmpresaId { get; private set; }

    public static Result<ImputacionCoste> Crear(
        TipoCoste tipo, OrigenImputacion origen,
        decimal cantidad, decimal precioUnitario, Guid empresaId,
        Guid? ordenFabricacionId = null, Guid? ordenTrabajoId = null,
        Guid? productoId = null, string? concepto = null,
        DateTimeOffset? fecha = null)
    {
        var importe = cantidad * precioUnitario;
        if (importe <= 0)
            return Result<ImputacionCoste>.Failure(ImputacionCosteErrors.ImporteInvalido);

        return Result<ImputacionCoste>.Success(new ImputacionCoste
        {
            Id = new ImputacionCosteId(Guid.NewGuid()),
            Tipo = tipo,
            Origen = origen,
            Cantidad = cantidad,
            PrecioUnitario = precioUnitario,
            Importe = importe,
            OrdenFabricacionId = ordenFabricacionId,
            OrdenTrabajoId = ordenTrabajoId,
            ProductoId = productoId,
            Concepto = concepto,
            Fecha = fecha ?? DateTimeOffset.UtcNow,
            EmpresaId = empresaId
        });
    }
}
