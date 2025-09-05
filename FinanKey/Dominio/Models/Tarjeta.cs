using SQLite;

namespace FinanKey.Dominio.Models
{
    [Table("Tarjeta")]
    public class Tarjeta
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [MaxLength(100), NotNull]
        public string Nombre { get; set; } = string.Empty;   // Ej: Walmart

        [MaxLength(4), NotNull]
        public string Ultimos4Digitos { get; set; } = string.Empty; // Ej: 1234

        [MaxLength(10), NotNull]
        public string Tipo { get; set; } = "Credito"; // "Credito" o "Debito"

        [MaxLength(30)]
        public string? Banco { get; set; }  // Ej: BCR

        [MaxLength(7)]
        public string? Vencimiento { get; set; } // Ej: "09/25"

        // Solo aplica si es crédito
        public double? LimiteCredito { get; set; }

        // Solo aplica si es débito
        public double? MontoInicial { get; set; }

        [MaxLength(20)]
        public string? Categoria { get; set; } // Visa, Mastercard, Amex
        public string? Logo { get; set; } // Ej: "icono_visa.svg"

        public string? Descripcion { get; set; }

        [MaxLength(10)]
        public string? ColorHex1 { get; set; } // Ej: "#FF5733"
        [MaxLength(10)]
        public string? ColorHex2 { get; set; } // Ej: "#FF5733"
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
    }
}
