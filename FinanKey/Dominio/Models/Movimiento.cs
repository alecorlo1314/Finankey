using SQLite;
namespace FinanKey.Dominio.Models
{
    [Table("Movimiento")]
    public class Movimiento
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; } 

        [NotNull]
        public string TipoMovimiento { get; set; }

        [NotNull]
        public double Monto { get; set; }
        public string? Descripcion { get; set; }
        public string? MedioPago { get; set; } = Enums.MedioPago.Efectivo.ToString();
        public bool? EsRecurrente { get; set; }
        public string? Frecuencia { get; set; } = Enums.Frecuencia.Mensual.ToString();
        [NotNull]
        public DateTime Fecha { get; set; }
        [NotNull]
        public int? CategoriaId { get; set; }
        [NotNull]
        public string? Comercio { get; set; }
        [NotNull]

        public int? TarjetaId { get; set; } // Seria como la FK para la tarjeta de crédito o debito

        public bool? EsPagado { get; set; } // Solo aplica a gastos con tarjeta de crédito

        public DateTime FechaCreacion { get; set; } = DateTime.Now;
    }
}
