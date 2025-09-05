
using SQLite;

namespace FinanKey.Dominio.Models
{
    public class Transacciones 
    {
        public decimal Monto { get; set; }
        public string? Descripcion { get; set; } 
        public string? TipoMovimiento { get; set; }
        public string TipoCuenta { get; set; }
        public Cuenta? Cuenta { get; set; }
        public string TipoCategoria { get; set; }
        public Categoria? Categoria { get; set; }
        public DateTime Fecha { get; set; }
        public string? ColorTransaccion { get; set; }
    }
}
