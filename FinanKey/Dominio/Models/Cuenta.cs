
using SQLite;

namespace FinanKey.Dominio.Models
{
    public class Cuenta
    {
        public int Id { get; set; }
        public string? NombreCuenta { get; set; }
        public string? NombreEntidadFinanciera { get; set; }
        public float Saldo { get; set; }
        public int IDTipoCuenta { get; set; }
        [Ignore] //para que se ignore en la base de datos SQLite
        public TipoCuenta? TipoCuenta { get; set; }
    }
}
