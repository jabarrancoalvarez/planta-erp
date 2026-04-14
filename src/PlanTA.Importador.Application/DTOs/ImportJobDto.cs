using PlanTA.Importador.Domain.Enums;

namespace PlanTA.Importador.Application.DTOs;

public record ImportJobDto(
    Guid Id, TipoImportacion Tipo, FormatoArchivo Formato, EstadoImportJob Estado,
    string NombreArchivo, int FilasTotales, int FilasValidas, int FilasConError,
    int FilasImportadas, string? ResumenErrores,
    DateTimeOffset? IniciadoEn, DateTimeOffset? FinalizadoEn);

public record ValidacionFilaDto(int NumeroFila, bool Valida, string? Error, Dictionary<string, string?> Campos);

public record ResultadoValidacionDto(
    Guid JobId, int TotalFilas, int FilasValidas, int FilasConError,
    List<ValidacionFilaDto> PreviewFilas);

public record PagedResult<T>(List<T> Items, int TotalCount, int Page, int PageSize)
{
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
}
