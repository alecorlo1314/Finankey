using FinanKey.Dominio.Models;
using FinanKey.Dominio.Interfaces;
using SQLite;
using System.Collections.ObjectModel;

namespace FinanKey.Infraestructura.Repositorios
{
    class ContextoDatosCuenta : IServicioTransaccionCuenta
    {
        SQLiteAsyncConnection? _baseDatos;
        async Task Init()
        {
            if (_baseDatos is not null) return;
            _baseDatos = new SQLiteAsyncConnection(Constantes.RutaBaseDatos, Constantes.Flags);
            var resultado = await _baseDatos.CreateTableAsync<Cuenta>();
        }
        public async Task<bool> CrearCuentaAsync(Cuenta cuenta)
        {
            await Init();
            var resultado = await _baseDatos.InsertAsync(cuenta);
            return resultado > 0;
        }
        public async Task<ObservableCollection<Cuenta>> ObtenerCuentasAsync()
        {
            await Init();
            return new ObservableCollection<Cuenta>(await _baseDatos.Table<Cuenta>().ToListAsync());
        }
        public async Task<bool> ActualizarCuentaAsync(Cuenta cuenta)
        {
            await Init();
            var resultado = await _baseDatos.UpdateAsync(cuenta);
            return resultado > 0;
        }
        public async Task<bool> EliminarCuentaAsync(int id)
        {
            await Init();
            var cuenta = await _baseDatos.FindAsync<Cuenta>(id);
            if (cuenta != null)
            {
                var resultado = await _baseDatos.DeleteAsync(cuenta);
                return resultado > 0;
            }
            return false;
        }
    }
}
