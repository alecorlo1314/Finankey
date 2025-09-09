using FinanKey.Dominio.Models;
using FinanKey.Dominio.Interfaces;
using SQLite;

namespace FinanKey.Infraestructura.Repositorios
{
    public partial class RepositorioBaseDatos : IServicioBaseDatos, IDisposable
    {
        private SQLiteAsyncConnection? _conexion;
        private readonly SemaphoreSlim _iniciarBloqueo = new SemaphoreSlim(1, 1);
        private bool _disposed = false;

        /// <summary>
        /// Obtiene la conexión a la base de datos
        /// </summary>
        /// <returns></returns>
        /// <exception cref="ObjectDisposedException"></exception>
        /// <exception cref="Exception"></exception>
        public async Task<SQLiteAsyncConnection> ObtenerConexion()
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(RepositorioBaseDatos));

            if (_conexion != null)
                return _conexion;

            await _iniciarBloqueo.WaitAsync();
            try
            {
                // Double-check locking pattern
                if (_conexion != null)
                    return _conexion;

                System.Diagnostics.Debug.WriteLine($"Creando conexión a: {Constantes.RutaBaseDatos}");

                _conexion = new SQLiteAsyncConnection(Constantes.RutaBaseDatos, Constantes.Flags);

                // Habilita las claves foráneas (relaciones entre tablas)
                await _conexion.ExecuteAsync("PRAGMA foreign_keys = ON;");

                // Balance entre velocidad y seguridad
                await _conexion.ExecuteAsync("PRAGMA synchronous = NORMAL;");

                // Más memoria cache = mejor rendimiento
                await _conexion.ExecuteAsync("PRAGMA cache_size = 1000;");

                // Usa memoria RAM para archivos temporales
                await _conexion.ExecuteAsync("PRAGMA temp_store = MEMORY;");

                await MigrarAsync(_conexion);

                System.Diagnostics.Debug.WriteLine("Conexión establecida exitosamente");
                return _conexion;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error estableciendo conexión: {ex.Message}");
                _conexion?.CloseAsync();
                _conexion = null;
                throw new Exception($"Error al conectar con la base de datos: {ex.Message}", ex);
            }
            finally
            {
                _iniciarBloqueo.Release();
            }
        }

        /// <summary>
        /// Realiza la migración de la base de datos
        /// </summary>
        /// <param name="conexion"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        private static async Task MigrarAsync(SQLiteAsyncConnection conexion)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("Iniciando migración de tablas...");

                // Crear tablas en orden de dependencias
                await conexion.CreateTableAsync<CategoriaMovimiento>();
                System.Diagnostics.Debug.WriteLine("✓ Tabla CategoriaMovimiento creada");

                await conexion.CreateTableAsync<Tarjeta>();
                System.Diagnostics.Debug.WriteLine("✓ Tabla Tarjeta creada");

                await conexion.CreateTableAsync<Movimiento>();
                System.Diagnostics.Debug.WriteLine("✓ Tabla Movimiento creada");

                // Verificar que las tablas se crearon correctamente
                await VerificarEstructuraTablas(conexion);

                System.Diagnostics.Debug.WriteLine("Migración completada exitosamente");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error en migración: {ex.Message}\n{ex.StackTrace}");
                throw new Exception($"Error durante la migración de base de datos: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Verifica la estructura de las tablas
        /// </summary>
        /// <param name="conexion"></param>
        /// <returns></returns>
        private static async Task VerificarEstructuraTablas(SQLiteAsyncConnection conexion)
        {
            try
            {
                // Verificar CategoriaMovimiento
                var tablasInfo = await conexion.QueryAsync<dynamic>("SELECT name FROM sqlite_master WHERE type='table'");
                System.Diagnostics.Debug.WriteLine($"Tablas encontradas: {string.Join(", ", tablasInfo.Select(t => t.name))}");

                // Verificar estructura de CategoriaMovimiento específicamente
                var columnasCategoria = await conexion.QueryAsync<dynamic>("PRAGMA table_info(CategoriaMovimiento)");
                System.Diagnostics.Debug.WriteLine("=== ESTRUCTURA CategoriaMovimiento ===");
                foreach (var col in columnasCategoria)
                {
                    System.Diagnostics.Debug.WriteLine($"Columna: {col.name}, Tipo: {col.type}, NotNull: {col.notnull}, PK: {col.pk}");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error verificando estructura: {ex.Message}");
            }
        }

        /// <summary>
        /// Cierra la conexión
        /// </summary>
        /// <returns></returns>
        public async Task CerrarConexionAsync()
        {
            await _iniciarBloqueo.WaitAsync();
            try
            {
                if (_conexion != null)
                {
                    await _conexion.CloseAsync();
                    _conexion = null;
                    System.Diagnostics.Debug.WriteLine("Conexión cerrada");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error cerrando conexión: {ex.Message}");
            }
            finally
            {
                _iniciarBloqueo.Release();
            }
        }

        /// <summary>
        /// Elimina la base de datos
        /// </summary>
        /// <returns></returns>
        public async Task EliminarBaseDatosAsync()
        {
            try
            {
                await CerrarConexionAsync();

                if (File.Exists(Constantes.RutaBaseDatos))
                {
                    File.Delete(Constantes.RutaBaseDatos);
                    System.Diagnostics.Debug.WriteLine("Base de datos eliminada");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error eliminando base de datos: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Reconstruye la base de datos
        /// </summary>
        /// <returns></returns>
        public async Task ReconstruirBaseDatosAsync()
        {
            try
            {
                await EliminarBaseDatosAsync();
                _conexion = null;
                await ObtenerConexion(); // Esto recreará todo
                System.Diagnostics.Debug.WriteLine("Base de datos reconstruida");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error reconstruyendo base de datos: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Desconecta la base de datos
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Desconecta la base de datos
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed && disposing)
            {
                _conexion?.CloseAsync().Wait();
                _iniciarBloqueo?.Dispose();
                _disposed = true;
            }
        }

        /// <summary>
        /// Desconecta la base de datos
        /// </summary>
        ~RepositorioBaseDatos()
        {
            Dispose(false);
        }
    }
}