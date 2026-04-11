using PlanTA.SharedKernel;

namespace PlanTA.Inventario.Domain.ValueObjects;

public class SKU : ValueObject
{
    public string Value { get; }

    private SKU(string value) => Value = value;

    public static SKU Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value) || value.Length > 50)
            throw new ArgumentException("SKU inválido", nameof(value));
        return new SKU(value.Trim().ToUpperInvariant());
    }

    protected override IEnumerable<object> GetAtomicValues()
    {
        yield return Value;
    }

    public override string ToString() => Value;
}
