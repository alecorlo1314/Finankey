using FinanKey.Dominio.Models;

namespace FinanKey.Dominio.Interfaces
{
    public interface IServicioPago
    {
        public Task PagoCreditoGastoAsync(int IdMovimientoGasto, int IdTarejetaPago, decimal MontoPago, bool allowNegative);
        public Task PagoMuchosCreditoGastoAsync(IEnumerable<int> IdsMovimientosGasto, int IdTarejetaPago, bool allowNegative = false);
    }
}
