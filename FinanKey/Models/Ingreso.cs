
using SQLite;

namespace FinanKey.Models
{
    public class Ingreso
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public decimal Monto { get; set; }
        public string Descripcion { get; set; }
        public int CuentaId { get; set; }
        public int CategoriaId { get; set; }
        [Ignore]
        public Categoria TipoCategoria { get; set; }
        [Ignore]
        public Cuenta Cuenta { get; set; }
        public DateTime Fecha { get; set; }
        public string? Tipo => "Ingreso";
        public string? ColorIngreso => "Green";
    }
}
