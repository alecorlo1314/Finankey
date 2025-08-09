using SQLite;

namespace FinanKey.Models
{
    [Table("Tarjeta")]
    public class Tarjeta
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        [Indexed, NotNull]
        public string Alias { get; set; } = string.Empty;

        [NotNull]
        public string Tipo { get; set; } = "Credito"; // "Credito" | "Debito" | "Corriente"

        [MaxLength(4)]
        public string UltimosCuatroDigitos { get; set; } = string.Empty;

        public string Color { get; set; } = "#37265A"; // default color hex

        public decimal? LimiteCredito { get; set; }

        public decimal SaldoPendiente { get; set; } = 0m; // Solo para tarjetas de crédito

        public decimal SaldoActual { get; set; } = 0m; // Para tarjetas de débito y corrientes

        public int? DiaCorte { get; set; }
        public int? DiaPago { get; set; }

        public bool EsPredeterminada { get; set; } = false;

        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
    }
}
