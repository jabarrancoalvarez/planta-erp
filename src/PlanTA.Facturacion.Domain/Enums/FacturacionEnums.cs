namespace PlanTA.Facturacion.Domain.Enums;

public enum TipoFactura
{
    Ordinaria,
    Rectificativa,
    Simplificada,
    Abono
}

public enum EstadoFactura
{
    Borrador,
    Emitida,
    Firmada,
    EnviadaVerifactu,
    Aceptada,
    Rechazada,
    Anulada
}

public enum EstadoVerifactu
{
    NoEnviada,
    Pendiente,
    Enviada,
    Aceptada,
    RechazadaPorAEAT,
    Error
}
