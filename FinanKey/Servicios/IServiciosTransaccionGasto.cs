
using FinanKey.Models;
using System.Collections.ObjectModel;

namespace FinanKey.Servicios
{
    public interface IServiciosTransaccionGasto
    {
        public Task<bool> CrearTransaccionGastoAsync(Gasto transaccionGasto);
        public Task<List<Gasto>> ObtenerTransaccionesGastoAsync();

    }
}
