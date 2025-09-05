using FinanKey.Dominio.Models;

namespace FinanKey.Dominio.Interfaces
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
