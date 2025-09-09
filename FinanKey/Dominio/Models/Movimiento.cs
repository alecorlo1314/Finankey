using CommunityToolkit.Mvvm.ComponentModel;
using SQLite;
namespace FinanKey.Dominio.Models
{
    [Table("Movimiento")]
    public class Movimiento
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [NotNull]
        [Column("Tipo")]
        public string TipoMovimiento { get; set; } = "Gasto"; // "Ingreso" o "Gasto"

        [NotNull]
        public double Monto { get; set; }

        public string? Descripcion { get; set; }

        public string MedioPago { get; set; } = "Tarjeta"; // "Efectivo" o "Tarjeta"

        public bool EsRecurrente { get; set; } = false;

        public string Frecuencia { get; set; } = "Mensual"; // "Diario", "Semanal", "Mensual"

        [NotNull]
        public DateTime FechaMovimiento { get; set; } = DateTime.Now;

        [NotNull]
        public int CategoriaId { get; set; } // FK para categoria

        [NotNull]
        public string Comercio { get; set; } = string.Empty;

        [NotNull]
        public int TarjetaId { get; set; } // FK para tarjeta

        public bool? EsPagado { get; set; } // Solo para crédito, puede ser null

        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        public string? BorderFondoEstado;
      
        public string? ColorFuenteEstado; 

        // Propiedades de navegación (no mapeadas)
        [Ignore]
        public Tarjeta? Tarjeta { get; set; }

        [Ignore]
        public CategoriaMovimiento? Categoria_Movimiento { get; set; }
    }
}
