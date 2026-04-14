namespace PlanTA.Mantenimiento.Domain.Enums;

public enum TipoMantenimiento
{
    Preventivo = 1,
    Correctivo = 2,
    Predictivo = 3,
    Inspeccion = 4
}

public enum EstadoOT
{
    Borrador = 1,
    Planificada = 2,
    EnEjecucion = 3,
    EnEspera = 4,
    Completada = 5,
    Cancelada = 6
}

public enum PrioridadOT
{
    Baja = 1,
    Media = 2,
    Alta = 3,
    Urgente = 4
}

public enum FrecuenciaPlan
{
    Diaria = 1,
    Semanal = 2,
    Mensual = 3,
    Trimestral = 4,
    Semestral = 5,
    Anual = 6,
    PorHorasUso = 7,
    PorCiclos = 8
}
