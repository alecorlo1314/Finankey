

using FinanKey.Models;

namespace FinanKey.Servicios
{
    public interface IServiciosTransaccionIngreso
    {
        public Task<bool> CrearTransaccionIngresoAsync(Ingreso transaccionIngreso);
        public Task<List<Ingreso>> ObtenerTransaccionesIngresoAsync();
        public Task<bool> ActualizarTransaccionIngresoAsync(Ingreso transaccionIngreso);
        public Task<bool> EliminarTransaccionIngresoAsync(int id);
        public Task<List<Ingreso>> ObtenerTransaccionesIngresoPorCuentaAsync(int cuentaId);
    }
}
