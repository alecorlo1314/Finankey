using SQLite;
namespace FinanKey.Models
{
    public class TipoCuenta
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        [MaxLength(50)]
        public string? Descripcion { get; set; }
        public string? Icono { get; set; }
    }
}