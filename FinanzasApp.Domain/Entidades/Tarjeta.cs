using FinanzasApp.Domain.Enumeraciones;
using SQLite;

namespace FinanzasApp.Domain.Entidades;

/// <summary>
/// Entidad principal que representa una tarjeta financiera (crédito o débito).
/// </summary>
[Table("Tarjeta")]
public class Tarjeta
{
    // ── Clave primaria ───────────────────────────────
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    /// Nombre descriptivo de la tarjeta (ej: "Visa Banco Nacional")
    public string Nombre { get; set; } = string.Empty;

    /// Últimos 4 dígitos para identificación visual
    [MaxLength(4)]
    public string UltimosDigitos { get; set; } = string.Empty;

    /// Tipo de tarjeta: Crédito o Débito
    public TipoTarjeta Tipo { get; set; }

    /// Color en HEX para personalización visual
    public string ColorHex { get; set; } = "#3A86FF";

    /// Banco emisor de la tarjeta
    public string Banco { get; set; } = string.Empty;

    /// Límite de crédito (solo aplica en tarjetas de crédito)
    public decimal? LimiteCredito { get; set; }

    /// En crédito: deuda acumulada. En débito: dinero disponible
    public decimal SaldoActual { get; set; }

    /// Fecha en que se registró la tarjeta
    public DateTime FechaRegistro { get; set; } = DateTime.Now;

    /// true = activa, false = archivada (eliminación lógica)
    public bool EstaActiva { get; set; } = true;

    /// Red de la tarjeta (ej: Visa, MasterCard)
    public string RedTarjeta { get; set; } = "Visa";

    // ── Propiedades calculadas (no se guardan en BD) ─
    [Ignore]
    public decimal CreditoDisponible =>
        Tipo == TipoTarjeta.Credito && LimiteCredito.HasValue
            ? LimiteCredito.Value - SaldoActual : 0;

    [Ignore]
    public double PorcentajeUso =>
        Tipo == TipoTarjeta.Credito && LimiteCredito is > 0
            ? (double)(SaldoActual / LimiteCredito.Value) * 100 : 0;


    /// Lista de transacciones asociadas (se llena manualmente)
    [Ignore]
    public List<Transaccion> Transacciones { get; set; } = [];
}
