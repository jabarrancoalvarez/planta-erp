using PlanTA.SharedKernel;

namespace PlanTA.Compras.Domain.ValueObjects;

public class CondicionesPago : ValueObject
{
    public int DiasVencimiento { get; }
    public decimal DescuentoProntoPago { get; }
    public string MetodoPago { get; }

    private CondicionesPago() => MetodoPago = string.Empty;

    public CondicionesPago(int diasVencimiento, decimal descuentoProntoPago, string metodoPago)
    {
        if (diasVencimiento < 0)
            throw new ArgumentException("Los dias de vencimiento no pueden ser negativos", nameof(diasVencimiento));
        if (descuentoProntoPago < 0 || descuentoProntoPago > 100)
            throw new ArgumentException("El descuento debe estar entre 0 y 100", nameof(descuentoProntoPago));
        if (string.IsNullOrWhiteSpace(metodoPago))
            throw new ArgumentException("El metodo de pago es obligatorio", nameof(metodoPago));

        DiasVencimiento = diasVencimiento;
        DescuentoProntoPago = descuentoProntoPago;
        MetodoPago = metodoPago;
    }

    protected override IEnumerable<object> GetAtomicValues()
    {
        yield return DiasVencimiento;
        yield return DescuentoProntoPago;
        yield return MetodoPago;
    }
}
