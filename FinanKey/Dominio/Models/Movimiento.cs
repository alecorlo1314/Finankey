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
        public string TipoMovimiento { get; set; } = Enums.TipoMovimiento.Gasto.ToString();

        [NotNull]
        public double Monto { get; set; }

        public string? Descripcion { get; set; }

        public string MedioPago { get; set; } = Enums.MedioPago.Tarjeta.ToString();

        public bool EsRecurrente { get; set; } = false;

        public string Frecuencia { get; set; } = Enums.Frecuencia.Mensual.ToString();

        [NotNull]
        public DateTime Fecha { get; set; } = DateTime.Now;

        [NotNull]
        public int CategoriaId { get; set; }

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
        public Categoria? Categoria { get; set; }
    }
}
