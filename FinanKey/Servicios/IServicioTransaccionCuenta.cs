using FinanKey.Models;
using System.Collections.ObjectModel;

namespace FinanKey.Servicios
{
    public interface IServicioTransaccionCuenta
    {
        public Task<bool> CrearCuentaAsync(Cuenta cuenta);
        public Task<ObservableCollection<Cuenta>> ObtenerCuentasAsync();
        public Task<bool> ActualizarCuentaAsync(Cuenta cuenta);
        public Task<bool> EliminarCuentaAsync(int id);
    }
}
