using PlanTA.CRM.Domain.Enums;

namespace PlanTA.CRM.Application.DTOs;

public record LeadListDto(
    Guid Id, string Nombre, string? Empresa, string? Email, string? Telefono,
    OrigenLead Origen, EstadoLead Estado, Guid? AsignadoAUserId, string? Notas);

public record OportunidadListDto(
    Guid Id, string Titulo, Guid? ClienteId, FaseOportunidad Fase,
    decimal ImporteEstimado, int ProbabilidadPct, decimal ValorPonderado,
    DateTimeOffset? FechaCierreEstimada, string? Descripcion);

public record ActividadCrmDto(
    Guid Id, TipoActividadCrm Tipo, string Asunto, string? Detalle,
    Guid? LeadId, Guid? OportunidadId,
    DateTimeOffset Fecha, DateTimeOffset? VencimientoEn, bool Completada);

public record PagedResult<T>(List<T> Items, int TotalCount, int Page, int PageSize)
{
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
}
