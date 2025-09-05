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
        public string? Descripcion { get; set; }
        [NotNull]
        public DateTime Fecha { get; set; }

        public int? CategoriaId { get; set; }

        public string? Comercio { get; set; }

        public int? TarjetaId { get; set; } // Seria como la FK para la tarjeta de crédito o debito

        public bool? EsPagado { get; set; }        // Solo aplica a gastos con tarjeta de crédito

        public DateTime FechaCreacion { get; set; } = DateTime.Now;
    }
}
