namespace PlanTA.Produccion.Domain.ValueObjects;

public record CantidadPlanificada
{
    public decimal Cantidad { get; }
    public string UnidadMedida { get; }

    public CantidadPlanificada(decimal cantidad, string unidadMedida)
    {
        if (cantidad <= 0)
            throw new ArgumentException("La cantidad planificada debe ser mayor que cero", nameof(cantidad));
        if (string.IsNullOrWhiteSpace(unidadMedida))
            throw new ArgumentException("La unidad de medida es obligatoria", nameof(unidadMedida));

        Cantidad = cantidad;
        UnidadMedida = unidadMedida.Trim();
    }
}
