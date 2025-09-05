using SQLite;

namespace FinanKey.Dominio.Models
{
    public class TipoCategoria
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        [MaxLength(50)]
        public string? Descripcion { get; set; }
        public string? Icono { get; set; } //Icono que se tomara de repositorio 
    }
}