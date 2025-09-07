using FinanKey.Dominio.Models;
using FinanKey.Dominio.Interfaces;
using SQLite;

namespace FinanKey.Infraestructura.Repositorios
{
    public class RepositorioBaseDatos 
    {
        private SQLiteAsyncConnection? _conexion;
        private readonly SemaphoreSlim _iniciarBloqueo = new SemaphoreSlim(1, 1);

        public async Task<SQLiteAsyncConnection> ObtenerConexion()
        {
            if (_conexion != null) return _conexion;

            await _iniciarBloqueo.WaitAsync();
            try
            {
                if (_conexion != null) return _conexion;

                _conexion = new SQLiteAsyncConnection(Constantes.RutaBaseDatos, Constantes.Flags);

                await _conexion.ExecuteAsync("PRAGMA foreign_keys = ON;");

                await MigrarAsync(_conexion);

                return _conexion;
            }
            finally
            {
                _iniciarBloqueo.Release();
            }
        }
        private static async Task MigrarAsync(SQLiteAsyncConnection conexion)
        {
                try
            {
                await conexion.CreateTableAsync<Tarjeta>();
                await conexion.CreateTableAsync<Movimiento>();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error creando tablas: {ex.Message}\n{ex.StackTrace}");
                throw;
            }
        }
        
        public async Task CorrerEnTransicionAsync(Func<SQLiteAsyncConnection, Task> trabajo)
        {
            //Obtenemos la conexión a la base de datos
            var conn = await ObtenerConexion();
            try
            {
                //Ejecutamos una transacción en modo inmediato para evitar bloqueos
                await conn.ExecuteAsync("BEGIN IMMEDIATE;");
                //Ejecutamos el trabajo que se le pasa como parámetro
                await trabajo(conn);
                //Si todo sale bien
                await conn.ExecuteAsync("COMMIT;");
            }
            catch
            {
                //Si ocurre un error, hacemos rollback de la transacción
                try { await conn.ExecuteAsync("ROLLBACK;"); } catch { }
                //Re-lanzamos la excepción para que el llamador pueda manejarla
                throw;
            }
        }
    }
}
