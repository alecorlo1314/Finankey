
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
        public double Monto { get; set; } //Ejemplo: 1000 o 5000 y asi

        public string? Descripcion { get; set; } //descripcion breve sobre los que se ingresara

        public string? MedioPago { get; set; } = "Tarjeta"; // "Efectivo" o "Tarjeta"

        public bool? EsRecurrente { get; set; } // hasta el momento no se usa

        public string? Frecuencia { get; set; } // "Diario", "Semanal", "Mensual" aun no se usa

        [NotNull]
        public DateTime FechaMovimiento { get; set; } = DateTime.Now; //Defaul el dia de hoy

        [NotNull]
        public int CategoriaId { get; set; } // FK para categoria

        [NotNull]
        public string? Comercio { get; set; }//Ejmplo, Pali, Walmart, superCompro

        [NotNull]
        public int TarjetaId { get; set; } // FK para tarjeta

        public bool? EsPagado { get; set; } = false; //Default se pone que aun no esta pagado

        public DateTime FechaCreacion { get; set; } = DateTime.Now;//La fecha en la que se hizo el registro en la base de datos

        public string? BorderFondoEstado { get; set; } //Color de fondo del estado
      
        public string? ColorFuenteEstado { get; set; } //Color del texto del estado

        // Propiedades de navegación (no mapeadas)
        [Ignore]
        public Tarjeta? Tarjeta { get; set; }//No se ingresan en la base de datos

        [Ignore]
        public CategoriaMovimiento? Categoria_Movimiento { get; set; }//No se ingresa en la base de datos
    }
}
