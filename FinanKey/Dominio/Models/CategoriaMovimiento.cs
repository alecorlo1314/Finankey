using SQLite;

namespace FinanKey.Dominio.Models
{
    [Table("CategoriaMovimiento")]
    public class CategoriaMovimiento
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }      
        [Indexed,Unique, NotNull]
        public string? Descripcion { get; set; } //Ejemplo: Comida, Transporte, Entretenimiento
        public int Icon_id { get; set; } //id del icono seleccionado
        public string? RutaIcono { get; set; }//Ruta del icono seleccionado
        public string? TipoMovimiento { get; set; }//Ejemplo: Gasto o Ingreso
        [Ignore]
        public Icono? Icono { get; set; }//No se guarda en la base de datos
    }
}
