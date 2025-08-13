using FinanKey.Models;

namespace FinanKey.Servicios
{
    public interface IServicioMovimiento
    {
        public Task<int> CrearMovimientoAsync(Movimiento NuevoMovimiento);
        public Task ActualizarMovimientoAsync(Movimiento MovimientoActualizado);
        public Task EliminarMovimientoAsync(Movimiento EliminarMovimiento);
        public Task<List<Movimiento>> ObtenerMovimientosAsync();
        public Task<Movimiento?> ObtenerMovimientoPorIdAsync(int IdMovimiento);
        public Task<List<Movimiento>> ListaCreditosPendientesAsync();
        public Task<List<Movimiento>> ListaGastosPorTarjetaAsync(int TarjetaId);
    }
}
