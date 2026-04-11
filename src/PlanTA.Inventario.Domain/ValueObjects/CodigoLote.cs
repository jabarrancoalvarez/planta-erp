using PlanTA.SharedKernel;

namespace PlanTA.Inventario.Domain.ValueObjects;

public class CodigoLote : ValueObject
{
    public string Value { get; }

    private CodigoLote(string value) => Value = value;

    public static CodigoLote Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Código de lote inválido", nameof(value));
        return new CodigoLote(value.Trim().ToUpperInvariant());
    }

    public static CodigoLote Generate(string prefijo = "LOT")
    {
        var fecha = DateTime.UtcNow.ToString("yyyy-MM");
        var secuencia = Guid.NewGuid().ToString("N")[..6].ToUpperInvariant();
        return new CodigoLote($"{prefijo}-{fecha}-{secuencia}");
    }

    protected override IEnumerable<object> GetAtomicValues()
    {
        yield return Value;
    }

    public override string ToString() => Value;
}
