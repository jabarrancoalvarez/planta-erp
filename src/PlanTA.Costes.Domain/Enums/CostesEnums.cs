namespace PlanTA.Costes.Domain.Enums;

public enum TipoCoste
{
    Material,
    ManoObra,
    Maquina,
    Subcontratacion,
    Indirecto,
    Otro
}

public enum OrigenImputacion
{
    Manual,
    FichajeOperario,
    ConsumoStock,
    ParteProduccion,
    OrdenTrabajo
}
