using FinanKey.Dominio.Interfaces;
using FinanKey.Dominio.Models;
using SQLite;

namespace FinanKey.Infraestructura.Repositorios
{
    public class ContextoDatosGasto : IServiciosTransaccionGasto
    {
        SQLiteAsyncConnection _baseDatos;

        async Task Init()
        {
            if (_baseDatos is not null) return;

            _baseDatos = new SQLiteAsyncConnection(Constantes.RutaBaseDatos, Constantes.Flags);
            var resultado = await _baseDatos.CreateTableAsync<Gasto>();
        }
        public async Task<bool> CrearTransaccionGastoAsync(Gasto transaccionGasto)
        {
            await Init();
            var resultado = await _baseDatos.InsertAsync(transaccionGasto);
            return resultado > 0;
        }
        public async Task<List<Gasto>> ObtenerTransaccionesGastoAsync()
        {
            await Init();
            return await _baseDatos.Table<Gasto>().ToListAsync();
        }
        public async Task<List<Gasto>> ObtenerTransaccionesGastoPorCuentaAsync(int idCuenta)
        {
            await Init();
            return await _baseDatos.Table<Gasto>().Where(g => g.CuentaId == idCuenta).ToListAsync();
        }
    }
}