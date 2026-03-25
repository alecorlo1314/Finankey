using FinanzasApp.Domain.Enumeraciones;
using SQLite;

namespace FinanzasApp.Domain.Entidades;

/// <summary>
/// Entidad principal que representa una tarjeta financiera (crédito o débito).
/// Los atributos SQLite-net se colocan aquí por pragmatismo en apps móviles.
/// </summary>
[Table("Tarjeta")]
public class Tarjeta
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    /// <summary>Nombre descriptivo de la tarjeta (ej: "Visa Banco Nacional")</summary>
    public string Nombre { get; set; } = string.Empty;

    /// <summary>Últimos 4 dígitos para identificación visual</summary>
    [MaxLength(4)]
    public string UltimosDigitos { get; set; } = string.Empty;

    /// <summary>Tipo: Credito=0 o Debito=1</summary>
    public TipoTarjeta Tipo { get; set; }

    /// <summary>Color de fondo en HEX para personalización visual</summary>
    public string ColorHex { get; set; } = "#3A86FF";

    public string Banco { get; set; } = string.Empty;

    /// <summary>Límite de crédito. Null en tarjetas de débito</summary>
    public decimal? LimiteCredito { get; set; }

    /// <summary>En crédito: deuda acumulada. En débito: dinero disponible</summary>
    public decimal SaldoActual { get; set; }

    public DateTime FechaRegistro { get; set; } = DateTime.Now;

    /// <summary>false = archivada (eliminación lógica)</summary>
    public bool EstaActiva { get; set; } = true;

    public string RedTarjeta { get; set; } = "Visa";

    // ── Propiedades calculadas — [Ignore] indica a SQLite-net que NO las persista

    [Ignore]
    public decimal CreditoDisponible =>
        Tipo == TipoTarjeta.Credito && LimiteCredito.HasValue
            ? LimiteCredito.Value - SaldoActual : 0;

    [Ignore]
    public double PorcentajeUso =>
        Tipo == TipoTarjeta.Credito && LimiteCredito is > 0
            ? (double)(SaldoActual / LimiteCredito.Value) * 100 : 0;

    /// <summary>Navigation property — se llena manualmente, SQLite-net la ignora</summary>
    [Ignore]
    public List<Transaccion> Transacciones { get; set; } = [];
}
