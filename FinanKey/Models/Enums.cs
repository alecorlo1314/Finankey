
namespace FinanKey.Models
{
    public class Enums
    {
        public enum TipoTarjeta
        {
            Debito = 0,
            Credito = 1,
        }
        public enum TipoMovimiento
        {
            Ingreso = 0,
            Gasto = 1,
        }
        public enum MarcaTarjeta
        {
            Visa = 0,
            Mastercard = 1,
            American_Express = 2,
        }
    }
}
