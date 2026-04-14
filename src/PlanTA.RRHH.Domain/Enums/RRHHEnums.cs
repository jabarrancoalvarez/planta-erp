namespace PlanTA.RRHH.Domain.Enums;

public enum TipoFichaje
{
    EntradaJornada = 1,
    SalidaJornada = 2,
    InicioDescanso = 3,
    FinDescanso = 4,
    InicioMaquina = 5,
    FinMaquina = 6,
    InicioOF = 7,
    FinOF = 8
}

public enum TipoAusencia
{
    Vacaciones = 1,
    BajaMedica = 2,
    AsuntoPropio = 3,
    Formacion = 4,
    Maternidad = 5,
    Paternidad = 6,
    Otro = 99
}

public enum EstadoAusencia
{
    Solicitada = 1,
    Aprobada = 2,
    Rechazada = 3,
    EnDisfrute = 4,
    Finalizada = 5
}
