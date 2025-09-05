using FinanKey.Dominio.Models;
using System.Collections.ObjectModel;

namespace FinanKey.Dominio.Interfaces
{
    public interface IServicioTransaccionCuenta
    {
        public Task<bool> CrearCuentaAsync(Cuenta cuenta);
        public Task<ObservableCollection<Cuenta>> ObtenerCuentasAsync();
        public Task<bool> ActualizarCuentaAsync(Cuenta cuenta);
        public Task<bool> EliminarCuentaAsync(int id);
    }
}
