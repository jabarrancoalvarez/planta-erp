namespace PlanTA.Incidencias.Domain.Enums;

public enum TipoIncidencia
{
    Averia = 1,
    Parada = 2,
    Seguridad = 3,
    CasiAccidente = 4,
    Calidad = 5,
    Otro = 99
}

public enum SeveridadIncidencia
{
    Baja = 1,
    Media = 2,
    Alta = 3,
    Critica = 4
}

public enum EstadoIncidencia
{
    Abierta = 1,
    EnRevision = 2,
    EnReparacion = 3,
    Resuelta = 4,
    Cerrada = 5,
    Descartada = 6
}
