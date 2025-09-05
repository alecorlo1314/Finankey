
namespace FinanKey.Dominio.Models
{
    public class Enums
    {
        public enum TipoTarjeta
        {
            Debito,
            Credito,
        }
        public enum TipoMovimiento
        {
            Ingreso,
            Gasto,
        }
        public enum MarcaTarjeta
        {
            Visa,
            Mastercard,
            American_Express,
        }
        public enum MedioPago
        {
            Efectivo,
            Tarjeta,
        }
        public enum Frecuencia
        {
            Diario,
            Semanal,
            Quincenal,
            Mensual,
            Anual,
        }
    }
}
