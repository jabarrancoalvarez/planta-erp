namespace PlanTA.Importador.Domain.Enums;

public enum TipoImportacion
{
    Productos,
    Clientes,
    Proveedores,
    Empleados,
    Activos,
    Stock
}

public enum EstadoImportJob
{
    Pendiente,
    Validando,
    ListaParaImportar,
    Importando,
    Completada,
    CompletadaConErrores,
    Fallida,
    Cancelada
}

public enum FormatoArchivo
{
    Csv,
    Xlsx
}
