using PlanTA.SharedKernel;

namespace PlanTA.Produccion.Domain.ValueObjects;

public class CodigoOF : ValueObject
{
    public string Value { get; }

    private CodigoOF(string value) => Value = value;

    public static CodigoOF Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Codigo OF no puede estar vacio", nameof(value));
        return new CodigoOF(value.Trim().ToUpperInvariant());
    }

    public static CodigoOF Generate(DateTimeOffset fecha, int secuencia)
    {
        var codigo = $"OF-{fecha:yyyy-MM}-{secuencia:D4}";
        return new CodigoOF(codigo);
    }

    protected override IEnumerable<object> GetAtomicValues()
    {
        yield return Value;
    }

    public override string ToString() => Value;
}
