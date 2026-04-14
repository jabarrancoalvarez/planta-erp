using PlanTA.IA.Domain.Enums;
using PlanTA.SharedKernel;

namespace PlanTA.IA.Domain.Entities;

public class MensajeIA : BaseEntity<MensajeIAId>
{
    private MensajeIA() { }

    public ConversacionIAId ConversacionId { get; private set; } = default!;
    public RolMensaje Rol { get; private set; }
    public string Contenido { get; private set; } = string.Empty;
    public int? TokensEntrada { get; private set; }
    public int? TokensSalida { get; private set; }
    public string? Modelo { get; private set; }

    public static MensajeIA Crear(
        ConversacionIAId conversacionId, RolMensaje rol, string contenido,
        int? tokensEntrada = null, int? tokensSalida = null, string? modelo = null)
        => new()
        {
            Id = new MensajeIAId(Guid.NewGuid()),
            ConversacionId = conversacionId,
            Rol = rol,
            Contenido = contenido,
            TokensEntrada = tokensEntrada,
            TokensSalida = tokensSalida,
            Modelo = modelo
        };
}
