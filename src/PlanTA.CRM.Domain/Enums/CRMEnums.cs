namespace PlanTA.CRM.Domain.Enums;

public enum EstadoLead
{
    Nuevo,
    Contactado,
    Calificado,
    Descartado,
    Convertido
}

public enum FaseOportunidad
{
    Prospecto,
    Contactado,
    PropuestaEnviada,
    Negociacion,
    Ganada,
    Perdida
}

public enum TipoActividadCrm
{
    Llamada,
    Email,
    Reunion,
    Tarea,
    Nota
}

public enum OrigenLead
{
    Web,
    Recomendacion,
    Evento,
    LlamadaFria,
    Marketing,
    Otro
}
