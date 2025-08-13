using SQLite;

namespace FinanKey.Models
{
    [Table("Categoria")]
    public class Categoria
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        [Indexed,Unique, NotNull]
        public string? Nombre { get; set; }
        public string Icono { get; set; } = "compras";
    }
}
