using FinanKey.Dominio.Models;
using FinanKey.Dominio.Interfaces;
using SQLite;

namespace FinanKey.Infraestructura.Repositorios
{
    public class ContextoDatosIngreso : IServiciosTransaccionIngreso
    {
        SQLiteAsyncConnection _baseDatos;

        async Task Init()
        {
            if (_baseDatos is not null) return;

            _baseDatos = new SQLiteAsyncConnection(Constantes.RutaBaseDatos, Constantes.Flags);
            var resultado = await _baseDatos.CreateTableAsync<Ingreso>();
        }
        public async Task<bool> CrearTransaccionIngresoAsync(Ingreso transaccionIngreso)
        {
            await Init();
            var resultado = await _baseDatos.InsertAsync(transaccionIngreso);
            return resultado > 0;
        }
        public async Task<List<Ingreso>> ObtenerTransaccionesIngresoAsync()
        {
            await Init();
            return await _baseDatos.Table<Ingreso>().ToListAsync();
        }
        public async Task<bool> ActualizarTransaccionIngresoAsync(Ingreso transaccionIngreso)
        {
            await Init();
            var resultado = await _baseDatos.UpdateAsync(transaccionIngreso);
            return resultado > 0;
        }
        public async Task<bool> EliminarTransaccionIngresoAsync(int id)
        {
            await Init();
            var transaccion = await _baseDatos.FindAsync<Ingreso>(id);
            if (transaccion != null)
            {
                var resultado = await _baseDatos.DeleteAsync(transaccion);
                return resultado > 0;
            }
            return false;
        }
        public async Task<List<Ingreso>> ObtenerTransaccionesIngresoPorCuentaAsync(int idCuenta)
        {
            await Init();
            return await _baseDatos.Table<Ingreso>().Where(g => g.CuentaId == idCuenta).ToListAsync();
        }
    }
}
