using FinanKey.Dominio.Models;

namespace FinanKey.Dominio.Interfaces
{
    public interface IServiciosTransaccionGasto
    {
        public Task<bool> CrearTransaccionGastoAsync(Gasto transaccionGasto);
        public Task<List<Gasto>> ObtenerTransaccionesGastoAsync();
        public Task<List<Gasto>> ObtenerTransaccionesGastoPorCuentaAsync(int cuentaId);

    }
}
