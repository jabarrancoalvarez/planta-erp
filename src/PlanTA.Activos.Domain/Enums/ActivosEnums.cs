namespace PlanTA.Activos.Domain.Enums;

public enum TipoActivo
{
    Maquina = 1,
    Linea = 2,
    Herramienta = 3,
    Vehiculo = 4,
    Instalacion = 5,
    Componente = 6
}

public enum CriticidadActivo
{
    Baja = 1,
    Media = 2,
    Alta = 3,
    Critica = 4
}

public enum EstadoActivo
{
    Operativo = 1,
    EnMantenimiento = 2,
    Averiado = 3,
    Parado = 4,
    Baja = 5
}
