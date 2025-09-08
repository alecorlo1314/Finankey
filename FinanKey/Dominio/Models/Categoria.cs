using SQLite;

namespace FinanKey.Dominio.Models
{
    [Table("CategoriaMovimiento")]
    public class CategoriaMovimiento
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        [Indexed,Unique, NotNull]
        public string? Descripcion { get; set; }
        public int? Icon_id { get; set; }
        [Ignore]
        public TipoCategoria? TipoCategoria { get; set; }
    }
}
