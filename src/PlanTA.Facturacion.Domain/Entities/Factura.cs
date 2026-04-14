using System.Security.Cryptography;
using System.Text;
using PlanTA.Facturacion.Domain.Enums;
using PlanTA.Facturacion.Domain.Errors;
using PlanTA.SharedKernel;

namespace PlanTA.Facturacion.Domain.Entities;

public class Factura : SoftDeletableEntity<FacturaId>
{
    private readonly List<LineaFactura> _lineas = new();

    private Factura() { }

    public string NumeroCompleto { get; private set; } = string.Empty;
    public string SerieCodigo { get; private set; } = string.Empty;
    public int Numero { get; private set; }
    public int Ejercicio { get; private set; }
    public TipoFactura Tipo { get; private set; }
    public EstadoFactura Estado { get; private set; } = EstadoFactura.Borrador;

    public Guid ClienteId { get; private set; }
    public string ClienteNombre { get; private set; } = string.Empty;
    public string? ClienteNIF { get; private set; }
    public string? ClienteDireccion { get; private set; }

    public DateTimeOffset FechaEmision { get; private set; }
    public DateTimeOffset? FechaVencimiento { get; private set; }

    public decimal BaseImponible { get; private set; }
    public decimal TotalIva { get; private set; }
    public decimal Total { get; private set; }
    public string? Observaciones { get; private set; }

    // Verifactu / Ley Antifraude
    public string? HashPrevio { get; private set; }
    public string? HashActual { get; private set; }
    public string? CodigoQrVerifactu { get; private set; }
    public EstadoVerifactu EstadoVerifactu { get; private set; } = EstadoVerifactu.NoEnviada;
    public DateTimeOffset? VerifactuEnviadaEn { get; private set; }
    public string? VerifactuRespuesta { get; private set; }
    public string? VerifactuCsv { get; private set; }

    public Guid EmpresaId { get; private set; }
    public IReadOnlyCollection<LineaFactura> Lineas => _lineas.AsReadOnly();

    public static Factura Crear(
        string serieCodigo, int numero, int ejercicio,
        Guid clienteId, string clienteNombre, Guid empresaId,
        TipoFactura tipo = TipoFactura.Ordinaria,
        string? clienteNif = null, string? clienteDireccion = null,
        DateTimeOffset? fechaEmision = null, DateTimeOffset? fechaVencimiento = null,
        string? observaciones = null)
        => new()
        {
            Id = new FacturaId(Guid.NewGuid()),
            SerieCodigo = serieCodigo,
            Numero = numero,
            Ejercicio = ejercicio,
            NumeroCompleto = $"{serieCodigo}/{ejercicio}/{numero:D6}",
            Tipo = tipo,
            ClienteId = clienteId,
            ClienteNombre = clienteNombre,
            ClienteNIF = clienteNif,
            ClienteDireccion = clienteDireccion,
            FechaEmision = fechaEmision ?? DateTimeOffset.UtcNow,
            FechaVencimiento = fechaVencimiento,
            Observaciones = observaciones,
            EmpresaId = empresaId
        };

    public Result<bool> AgregarLinea(
        string descripcion, decimal cantidad, decimal precioUnitario, decimal ivaPct,
        decimal descuentoPct = 0, Guid? productoId = null)
    {
        if (Estado != EstadoFactura.Borrador)
            return Result<bool>.Failure(FacturaErrors.EstadoInvalido(Estado.ToString()));

        var num = _lineas.Count + 1;
        _lineas.Add(LineaFactura.Crear(Id, num, descripcion, cantidad, precioUnitario, ivaPct, descuentoPct, productoId));
        Recalcular();
        MarkUpdated();
        return Result<bool>.Success(true);
    }

    private void Recalcular()
    {
        BaseImponible = _lineas.Sum(l => l.BaseImponible);
        TotalIva = _lineas.Sum(l => l.Iva);
        Total = BaseImponible + TotalIva;
    }

    public Result<bool> Emitir(string? hashPrevioFactura)
    {
        if (Estado != EstadoFactura.Borrador)
            return Result<bool>.Failure(FacturaErrors.EstadoInvalido(Estado.ToString()));
        if (_lineas.Count == 0)
            return Result<bool>.Failure(FacturaErrors.SinLineas);

        HashPrevio = hashPrevioFactura;
        HashActual = CalcularHash(hashPrevioFactura);
        Estado = EstadoFactura.Emitida;
        MarkUpdated();
        return Result<bool>.Success(true);
    }

    private string CalcularHash(string? hashPrevio)
    {
        var payload = $"{NumeroCompleto}|{FechaEmision:o}|{ClienteNIF}|{Total}|{hashPrevio ?? string.Empty}";
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(payload));
        return Convert.ToHexString(bytes);
    }

    public void MarcarEnviadaVerifactu(string csv, string respuesta)
    {
        EstadoVerifactu = EstadoVerifactu.Enviada;
        VerifactuCsv = csv;
        VerifactuRespuesta = respuesta;
        VerifactuEnviadaEn = DateTimeOffset.UtcNow;
        Estado = EstadoFactura.EnviadaVerifactu;
        CodigoQrVerifactu = $"https://sede.agenciatributaria.gob.es/verifactu?csv={csv}";
        MarkUpdated();
    }

    public void Anular()
    {
        Estado = EstadoFactura.Anulada;
        MarkUpdated();
    }
}
