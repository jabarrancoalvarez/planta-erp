namespace PlanTA.Seguridad.Domain.Entities;

public class Notificacion
{
    public Guid Id { get; private set; }
    public Guid EmpresaId { get; private set; }
    public Guid? UsuarioId { get; private set; }
    public string Titulo { get; private set; } = string.Empty;
    public string Mensaje { get; private set; } = string.Empty;
    public string Tipo { get; private set; } = "info";
    public string? Url { get; private set; }
    public bool Leida { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset? LeidaAt { get; private set; }

    private Notificacion() { }

    public static Notificacion Crear(Guid empresaId, string titulo, string mensaje, string tipo = "info", Guid? usuarioId = null, string? url = null)
    {
        return new Notificacion
        {
            Id = Guid.NewGuid(),
            EmpresaId = empresaId,
            UsuarioId = usuarioId,
            Titulo = titulo,
            Mensaje = mensaje,
            Tipo = tipo,
            Url = url,
            Leida = false,
            CreatedAt = DateTimeOffset.UtcNow,
        };
    }

    public void MarcarLeida()
    {
        Leida = true;
        LeidaAt = DateTimeOffset.UtcNow;
    }
}
