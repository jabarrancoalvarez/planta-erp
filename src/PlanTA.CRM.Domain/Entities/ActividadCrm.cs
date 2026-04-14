using PlanTA.CRM.Domain.Enums;
using PlanTA.SharedKernel;

namespace PlanTA.CRM.Domain.Entities;

public class ActividadCrm : BaseEntity<ActividadCrmId>
{
    private ActividadCrm() { }

    public TipoActividadCrm Tipo { get; private set; }
    public string Asunto { get; private set; } = string.Empty;
    public string? Detalle { get; private set; }
    public Guid? LeadId { get; private set; }
    public Guid? OportunidadId { get; private set; }
    public DateTimeOffset Fecha { get; private set; }
    public DateTimeOffset? VencimientoEn { get; private set; }
    public bool Completada { get; private set; }
    public Guid UsuarioId { get; private set; }
    public Guid EmpresaId { get; private set; }

    public static ActividadCrm Crear(
        TipoActividadCrm tipo, string asunto, Guid usuarioId, Guid empresaId,
        string? detalle = null, Guid? leadId = null, Guid? oportunidadId = null,
        DateTimeOffset? vencimientoEn = null)
        => new()
        {
            Id = new ActividadCrmId(Guid.NewGuid()),
            Tipo = tipo,
            Asunto = asunto.Trim(),
            Detalle = detalle,
            LeadId = leadId,
            OportunidadId = oportunidadId,
            Fecha = DateTimeOffset.UtcNow,
            VencimientoEn = vencimientoEn,
            UsuarioId = usuarioId,
            EmpresaId = empresaId
        };

    public void MarcarCompletada() { Completada = true; MarkUpdated(); }
}
