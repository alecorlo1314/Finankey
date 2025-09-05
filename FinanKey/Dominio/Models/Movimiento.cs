using SQLite;
namespace FinanKey.Dominio.Models
{
    [Table("Movimiento")]
    public class Movimiento
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [NotNull]
        public string Tipo { get; set; } = "Gasto"; // "Ingreso" | "Gasto"

        [NotNull]
        public decimal Monto { get; set; }

        [NotNull]
        public DateTime Fecha { get; set; } = DateTime.UtcNow;

        public int? CategoriaId { get; set; }

        public string? Comercio { get; set; }

        public int? TarjetaId { get; set; } // FK -> Card

        // Solo aplica a gastos con tarjeta de crédito
        public bool? EsPagado { get; set; }

        public int? MovimientoPagoId { get; set; } // FK to payment movement (gasto) from debit/corriente

        public string? Notas { get; set; }
    }
}
