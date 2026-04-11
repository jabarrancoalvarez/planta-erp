namespace PlanTA.Seguridad.Domain.Enums;

public static class RolSistema
{
    public const string Administrador = nameof(Administrador);
    public const string GerentePlanta = nameof(GerentePlanta);
    public const string JefeAlmacen = nameof(JefeAlmacen);
    public const string JefeProduccion = nameof(JefeProduccion);
    public const string Compras = nameof(Compras);
    public const string Ventas = nameof(Ventas);
    public const string Operario = nameof(Operario);
    public const string Calidad = nameof(Calidad);

    public static readonly string[] Todos =
    [
        Administrador, GerentePlanta, JefeAlmacen, JefeProduccion,
        Compras, Ventas, Operario, Calidad
    ];
}
