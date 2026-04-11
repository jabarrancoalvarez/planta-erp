using PlanTA.SharedKernel;

namespace PlanTA.Inventario.Domain.ValueObjects;

/// <summary>Código de ubicación en formato Pasillo-Estante-Nivel (ej: "A-03-2")</summary>
public class CodigoUbicacion : ValueObject
{
    public string Pasillo { get; }
    public string Estante { get; }
    public string Nivel { get; }

    private CodigoUbicacion(string pasillo, string estante, string nivel)
    {
        Pasillo = pasillo;
        Estante = estante;
        Nivel = nivel;
    }

    public static CodigoUbicacion Create(string pasillo, string estante, string nivel)
    {
        return new CodigoUbicacion(
            pasillo.Trim().ToUpperInvariant(),
            estante.Trim(),
            nivel.Trim());
    }

    public string CodigoCompleto => $"{Pasillo}-{Estante}-{Nivel}";

    protected override IEnumerable<object> GetAtomicValues()
    {
        yield return Pasillo;
        yield return Estante;
        yield return Nivel;
    }

    public override string ToString() => CodigoCompleto;
}
