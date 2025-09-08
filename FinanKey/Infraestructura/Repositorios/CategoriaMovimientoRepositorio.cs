
using FinanKey.Dominio.Interfaces;
using FinanKey.Dominio.Models;
using SQLite;

namespace FinanKey.Infraestructura.Repositorios
{
    public class CategoriaMovimientoRepositorio : IServicioCategoriaMovimiento
    {
        private readonly SQLiteAsyncConnection _database;

        public CategoriaMovimientoRepositorio(SQLiteAsyncConnection database)
        {
            _database = database;
        }

        /// <summary>
        /// Obtiene todas las categorías de movimiento
        /// </summary>
        public async Task<List<CategoriaMovimiento>> ObtenerTodosAsync()
        {
            try
            {
                return await _database.Table<CategoriaMovimiento>()
                                     .OrderBy(c => c.Descripcion)
                                     .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener todas las categorías: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Obtiene una categoría por su ID
        /// </summary>
        public async Task<CategoriaMovimiento?> ObtenerPorIdAsync(int id)
        {
            try
            {
                return await _database.Table<CategoriaMovimiento>()
                                     .Where(c => c.Id == id)
                                     .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener categoría por ID {id}: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Obtiene categorías filtradas por tipo de movimiento (Ingreso/Gasto)
        /// </summary>
        public async Task<List<CategoriaMovimiento>> ObtenerPorTipoMovimientoAsync(string tipoMovimiento)
        {
            try
            {
                return await _database.Table<CategoriaMovimiento>()
                                     .Where(c => c.TipoMovimiento == tipoMovimiento)
                                     .OrderBy(c => c.Descripcion)
                                     .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener categorías por tipo {tipoMovimiento}: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Obtiene una categoría por su descripción
        /// </summary>
        public async Task<CategoriaMovimiento?> ObtenerPorDescripcionAsync(string descripcion)
        {
            try
            {
                return await _database.Table<CategoriaMovimiento>()
                                     .Where(c => c.Descripcion == descripcion)
                                     .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener categoría por descripción {descripcion}: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Inserta una nueva categoría
        /// </summary>
        public async Task<int> InsertarAsync(CategoriaMovimiento categoriaMovimiento)
        {
            try
            {
                // Validar que no existe una categoría con la misma descripción
                var existe = await ExisteDescripcionAsync(categoriaMovimiento.Descripcion ?? string.Empty);
                if (existe)
                {
                    throw new InvalidOperationException($"Ya existe una categoría con la descripción '{categoriaMovimiento.Descripcion}'");
                }

                return await _database.InsertAsync(categoriaMovimiento);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al insertar categoría: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Actualiza una categoría existente
        /// </summary>
        public async Task<int> ActualizarAsync(CategoriaMovimiento categoriaMovimiento)
        {
            try
            {
                // Validar que no existe otra categoría con la misma descripción
                var existe = await ExisteDescripcionAsync(categoriaMovimiento.Descripcion ?? string.Empty, categoriaMovimiento.Id);
                if (existe)
                {
                    throw new InvalidOperationException($"Ya existe otra categoría con la descripción '{categoriaMovimiento.Descripcion}'");
                }

                return await _database.UpdateAsync(categoriaMovimiento);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al actualizar categoría: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Elimina una categoría por ID
        /// </summary>
        public async Task<int> EliminarAsync(int id)
        {
            try
            {
                return await _database.DeleteAsync<CategoriaMovimiento>(id);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al eliminar categoría con ID {id}: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Elimina una categoría por objeto
        /// </summary>
        public async Task<int> EliminarAsync(CategoriaMovimiento categoriaMovimiento)
        {
            try
            {
                return await _database.DeleteAsync(categoriaMovimiento);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al eliminar categoría: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Verifica si existe una categoría con la descripción especificada
        /// </summary>
        public async Task<bool> ExisteDescripcionAsync(string descripcion)
        {
            try
            {
                var count = await _database.Table<CategoriaMovimiento>()
                                          .Where(c => c.Descripcion == descripcion)
                                          .CountAsync();
                return count > 0;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al verificar existencia de descripción {descripcion}: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Verifica si existe una categoría con la descripción especificada, excluyendo un ID específico
        /// </summary>
        public async Task<bool> ExisteDescripcionAsync(string descripcion, int idExcluir)
        {
            try
            {
                var count = await _database.Table<CategoriaMovimiento>()
                                          .Where(c => c.Descripcion == descripcion && c.Id != idExcluir)
                                          .CountAsync();
                return count > 0;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al verificar existencia de descripción {descripcion} excluyendo ID {idExcluir}: {ex.Message}", ex);
            }
        }
    }
}

