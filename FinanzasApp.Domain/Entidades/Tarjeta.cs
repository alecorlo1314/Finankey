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


    //EL VENCIMIENTO SERA PARA AMBOS TIPOS DE TARJETAS
    public int MesVencimiento { get; set; }
    public int AnioVencimiento { get; set; }

    //EL DIA DE PAGO Y CORTE SERA SOLO PARA LA TARJETA DE CREDITO
    public int? DiaCorte { get; set; }
    public int? DiaPago { get; set; }


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


    /// <summary>
    /// Fecha exacta del próximo corte a partir de hoy.
    /// Si hoy es después del día de corte, el próximo corte
    /// es el mes siguiente.
    /// </summary>
    [Ignore]
    public DateTime? ProximoCorte
    {
        get
        {
            //Si esta vacio devulve null
            if(!DiaCorte.HasValue) return null;

            var hoy = DateTime.Today;
            var diaCorte = DiaCorte.Value;

            // Ajusta si el día no existe en el mes (ej: día 31 en febrero)
            var diasEnMesActual = DateTime.DaysInMonth(hoy.Year, hoy.Month);
            var diaReal = Math.Min(diaCorte, diasEnMesActual);

            var corteMesActual = new DateTime(hoy.Year, hoy.Month, diaReal);

            // Si el corte de este mes ya pasó, el próximo es el siguiente mes
            if (hoy > corteMesActual)
            {
                var mesSiguiente = hoy.AddMonths(1);
                var diasEnMesSiguiente = DateTime.DaysInMonth(mesSiguiente.Year, mesSiguiente.Month);
                diaReal = Math.Min(diaCorte, diasEnMesSiguiente);
                return new DateTime(mesSiguiente.Year, mesSiguiente.Month, diaReal);
            }
            return corteMesActual;
        }
    }

    /// <summary>
    /// Fecha exacta del próximo pago a partir del próximo corte.
    /// El pago siempre es el mes siguiente al corte.
    /// </summary>
    [Ignore]
    public DateTime? ProximoPago
    {
        get
        {
            if (!DiaPago.HasValue || !ProximoCorte.HasValue) return null;

            var mesCorte = ProximoCorte.Value.AddMonths(1);
            var diasEnMes = DateTime.DaysInMonth(mesCorte.Year, mesCorte.Month);
            var diaReal = Math.Min(DiaPago.Value, diasEnMes);

            return new DateTime(mesCorte.Year, mesCorte.Month, diaReal);
        }
    }

    /// <summary>
    /// Días restantes para el próximo corte desde hoy.
    /// </summary>
    [Ignore]
    public int? DiasParaCorte =>
        ProximoCorte.HasValue
            ? (ProximoCorte.Value - DateTime.Today).Days
            : null;

    /// <summary>
    /// Días restantes para el próximo pago desde hoy.
    /// </summary>
    [Ignore]
    public int? DiasParaPago =>
        ProximoPago.HasValue
            ? (ProximoPago.Value - DateTime.Today).Days
            : null;

    /// <summary>
    /// Fecha de vencimiento de la tarjeta física como DateTime.
    /// Usa el último día del mes de vencimiento.
    /// </summary>
    [Ignore]
    public DateTime? FechaVencimiento =>
        MesVencimiento > 0 && AnioVencimiento > 0
            ? new DateTime(
                AnioVencimiento,
                MesVencimiento,
                DateTime.DaysInMonth(AnioVencimiento, MesVencimiento))
            : null;

    /// <summary>
    /// Indica si la tarjeta física está vencida.
    /// </summary>
    [Ignore]
    public bool EstaVencida =>
        FechaVencimiento.HasValue && FechaVencimiento.Value < DateTime.Today;
}
