
using SQLite;

namespace FinanKey.Dominio.Models
{
    [Table("Icono")]
    public class Icono
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string? Nombre { get; set; }
        public string? Ruta { get; set; }
    }
}
