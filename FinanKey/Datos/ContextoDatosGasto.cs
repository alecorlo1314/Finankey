using FinanKey.Servicios;
using FinanKey.Models;
using SQLite;

namespace FinanKey.Datos
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
    }
}