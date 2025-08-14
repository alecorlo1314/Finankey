
using FinanKey.Models;  
using SQLite;

namespace FinanKey.Datos
{
    public class ServicioBaseDatos
    {
        private SQLiteAsyncConnection _conexion;
        private readonly SemaphoreSlim _iniciarBloqueo = new SemaphoreSlim(1, 1);

        public async Task<SQLiteAsyncConnection> ObtenerConexion()
        {
            //Si ya hay una conexión activa, la retornamos directamente
            if (_conexion != null) return _conexion;
            //Si no, esperamos a que se libere el bloqueo para iniciar la conexión
            await _iniciarBloqueo.WaitAsync();
            try
            {
                //Si ya hay una conexión activa, la retornamos directamente
                if (_conexion != null) return _conexion;
                //Iniciamos un nueva conexión a la base de datos con la ruta y las flags definidas en Constantes
                _conexion = new SQLiteAsyncConnection(Constantes.RutaBaseDatos, Constantes.Flags);
                //Activamos las claves foráneas para mantener la integridad referencial
                await _conexion.ExecuteAsync("PRAGMA foreign_keys = ON;");
                //Realizamos la migración de tablas y datos si es necesario
                await MigrarAsync(_conexion);
                //Retornamos la conexión activa
                return _conexion;
            }
            finally
            {
                _iniciarBloqueo.Release();
            }
        }
        private static async Task MigrarAsync(SQLiteAsyncConnection conexion)
        {
            // Verificar si la base de datos ya tiene las tablas necesarias
            await conexion.CreateTableAsync<Tarjeta>();
            await conexion.CreateTableAsync<Movimiento>();
            await conexion.CreateTableAsync<Categoria>();

            // Verificar si hay categorías predefinidas
            var count = await conexion.Table<Categoria>().CountAsync();
            //si no hay categorías, insertamos las categorías predefinidas
            if (count == 0)
            {
                // Definimos las categorías predefinidas
                var cats = new[]
                {
                "Compras","Comida","Transporte","Telefono","Ocio","Salud","Suscripciones","Educacion","Hogar","Otros","Salario","Freelance","Intereses","Reembolso"
            };
                foreach (var c in cats)
                    // Insertamos cada categoría en la base de datos
                    await conexion.InsertAsync(new Categoria { Nombre = c, Icon = "shopping" });
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
