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
        public string? Color { get; set; }
        public TipoCategoria TipoCategoria { get; set; }
    }
}
