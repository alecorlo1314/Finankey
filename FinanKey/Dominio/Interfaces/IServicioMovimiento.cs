using FinanKey.Dominio.Models;

namespace FinanKey.Dominio.Interfaces
{
    public interface IServicioMovimiento
    {
        public Task<int> AgregarMovimientoAsync(Movimiento NuevoMovimiento);
        public Task ActualizarMovimientoAsync(Movimiento MovimientoActualizado);
        public Task EliminarMovimientoAsync(Movimiento EliminarMovimiento);
        public Task<List<Movimiento>> ObtenerMovimientosAsync();
        public Task<Movimiento?> ObtenerMovimientoPorIdAsync(int IdMovimiento);
        public Task<List<Movimiento>> ListaCreditosPendientesAsync();
        public Task<List<Movimiento>> ListaGastosPorTarjetaAsync(int TarjetaId);
    }
}
