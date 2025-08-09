
using FinanKey.Models;  
using SQLite;

namespace FinanKey.Datos
{
    public class ServicioBaseDatos
    {
        private SQLiteAsyncConnection _conexion;
        private readonly SemaphoreSlim _iniciarBloqueo = new SemaphoreSlim(1, 1);
        private const SQLiteOpenFlags _banderas = SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.SharedCache;

        public async Task<SQLiteAsyncConnection> ObtenerConexion()
        {
            if (_conexion != null) return _conexion;
            await _iniciarBloqueo.WaitAsync();
            try
            {
                if (_conexion != null) return _conexion;

                var dbPath = Path.Combine(FileSystem.AppDataDirectory, "finankey.db3");
                _conexion = new SQLiteAsyncConnection(dbPath, _banderas);
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
            await conexion.CreateTableAsync<Tarjeta>();
            await conexion.CreateTableAsync<Movimiento>();
            await conexion.CreateTableAsync<Categoria>();

            // Verificar si hay categorías predefinidas
            var count = await conexion.Table<Categoria>().CountAsync();
            if (count == 0)
            {
                var cats = new[]
                {
                "Compras","Comida","Transporte","Telefono","Ocio","Salud","Suscripciones","Educacion","Hogar","Otros","Salario","Freelance","Intereses","Reembolso"
            };
                foreach (var c in cats)
                    await conexion.InsertAsync(new Categoria { Nombre = c, Icono = "shopping" });
            }
        }
        
        public async Task RunInTransactionAsync(Func<SQLiteAsyncConnection, Task> work)
        {
            var conn = await ObtenerConexion();
            try
            {
                await conn.ExecuteAsync("BEGIN IMMEDIATE;");
                await work(conn);
                await conn.ExecuteAsync("COMMIT;");
            }
            catch
            {
                try { await conn.ExecuteAsync("ROLLBACK;"); } catch { }
                throw;
            }
        }
    }
}
