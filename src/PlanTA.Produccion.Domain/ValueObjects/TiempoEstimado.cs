namespace PlanTA.Produccion.Domain.ValueObjects;

public record TiempoEstimado
{
    public decimal Minutos { get; }

    public TiempoEstimado(decimal minutos)
    {
        if (minutos < 0)
            throw new ArgumentException("El tiempo estimado no puede ser negativo", nameof(minutos));
        Minutos = minutos;
    }
}
