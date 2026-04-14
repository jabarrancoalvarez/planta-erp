using PlanTA.IA.Domain.Enums;
using PlanTA.SharedKernel;

namespace PlanTA.IA.Domain.Entities;

public class ConversacionIA : SoftDeletableEntity<ConversacionIAId>
{
    private readonly List<MensajeIA> _mensajes = new();

    private ConversacionIA() { }

    public string Titulo { get; private set; } = string.Empty;
    public ContextoIA Contexto { get; private set; } = ContextoIA.General;
    public Guid UsuarioId { get; private set; }
    public int TotalMensajes { get; private set; }
    public int TotalTokensEntrada { get; private set; }
    public int TotalTokensSalida { get; private set; }
    public Guid EmpresaId { get; private set; }

    public IReadOnlyCollection<MensajeIA> Mensajes => _mensajes.AsReadOnly();

    public static ConversacionIA Crear(string titulo, ContextoIA contexto, Guid usuarioId, Guid empresaId)
        => new()
        {
            Id = new ConversacionIAId(Guid.NewGuid()),
            Titulo = titulo.Trim(),
            Contexto = contexto,
            UsuarioId = usuarioId,
            EmpresaId = empresaId
        };

    public MensajeIA AgregarMensaje(RolMensaje rol, string contenido,
        int? tokensEntrada = null, int? tokensSalida = null, string? modelo = null)
    {
        var msg = MensajeIA.Crear(Id, rol, contenido, tokensEntrada, tokensSalida, modelo);
        _mensajes.Add(msg);
        TotalMensajes++;
        if (tokensEntrada.HasValue) TotalTokensEntrada += tokensEntrada.Value;
        if (tokensSalida.HasValue) TotalTokensSalida += tokensSalida.Value;
        MarkUpdated();
        return msg;
    }
}
