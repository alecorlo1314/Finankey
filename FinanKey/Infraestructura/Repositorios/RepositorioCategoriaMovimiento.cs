
using FinanKey.Dominio.Interfaces;
using FinanKey.Dominio.Models;
using SQLite;

namespace FinanKey.Infraestructura.Repositorios
{
    public class RepositorioCategoriaMovimiento : IServicioCategoriaMovimiento
    {
        private readonly RepositorioBaseDatos _servicioBaseDatos;

        public RepositorioCategoriaMovimiento(RepositorioBaseDatos servicioBaseDatos)
        {
            _servicioBaseDatos = servicioBaseDatos;
        }

        /// <summary>
        /// Obtiene todas las categorías de movimiento
        /// </summary>
        public async Task<List<CategoriaMovimiento>> ObtenerTodosAsync()
        {
            try
            {
                //Obtenemos la conexion a la base de datos
                var conexion = await _servicioBaseDatos.ObtenerConexion();

                var listaCategorias = await conexion.Table<CategoriaMovimiento>()
                                     .OrderBy(c => c.Descripcion)
                                     .ToListAsync();
                return listaCategorias;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener todas las categorías: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Metodo para Obtener categorias por gastos
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<List<CategoriaMovimiento>> ObtenerCategoriasTipoGastosAsync()
        {
            try
            {
                //Obtenemos la conexion a la base de datos
                var conexion = await _servicioBaseDatos.ObtenerConexion();

                var listaCategoriasGastos = await conexion.Table<CategoriaMovimiento>()
                                     .Where(cg => cg.TipoMovimiento == "Gasto")
                                     .OrderBy(c => c.Descripcion)
                                     .ToListAsync();
                return listaCategoriasGastos;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener categorias por Gastos: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Metodo para Obtener categorias por ingresos
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<List<CategoriaMovimiento>> ObtenerCategoriasTipoIngresosAsync()
        {
            try
            {
                //Obtenemos la conexion a la base de datos
                var conexion = await _servicioBaseDatos.ObtenerConexion();

                var listaCategoriasGastos = await conexion.Table<CategoriaMovimiento>()
                                     .Where(cg => cg.TipoMovimiento == "Ingreso")
                                     .OrderBy(c => c.Descripcion)
                                     .ToListAsync();
                return listaCategoriasGastos;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener categorias por Gastos: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Obtiene una categoría por su ID
        /// </summary>
        public async Task<CategoriaMovimiento?> ObtenerPorIdAsync(int id)
        {
            try
            {
                //Obtenemos la conexion a la base de datos
                var conexion = await _servicioBaseDatos.ObtenerConexion();
                return await conexion.Table<CategoriaMovimiento>()
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
                //Obtenemos la conexion a la base de datos
                var conexion = await _servicioBaseDatos.ObtenerConexion();

                //// Usar comparación más robusta
                var listaCategorias = await conexion.Table<CategoriaMovimiento>()
                                     .Where(c => c.TipoMovimiento != null &&
                                                c.TipoMovimiento.Equals(tipoMovimiento))
                                     .OrderBy(c => c.Descripcion)
                                     .ToListAsync();
                return listaCategorias;
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
                //Obtenemos la conexion a la base de datos
                var conexion = await _servicioBaseDatos.ObtenerConexion();
                return await conexion.Table<CategoriaMovimiento>()
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
                //Obtenemos la conexion a la base de datos
                var conexion = await _servicioBaseDatos.ObtenerConexion();

                // Validar que no existe una categoría con la misma descripción
                var existe = await ExisteDescripcionAsync(categoriaMovimiento.Descripcion ?? string.Empty);
                if (existe)
                {
                    throw new InvalidOperationException($"Ya existe una categoría con la descripción '{categoriaMovimiento.Descripcion}'");
                }

                return await conexion.InsertAsync(categoriaMovimiento);
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
                var conexion = await _servicioBaseDatos.ObtenerConexion();
                // Validar que no existe otra categoría con la misma descripción
                var existe = await ExisteDescripcionAsync(categoriaMovimiento.Descripcion ?? string.Empty, categoriaMovimiento.Id);
                if (existe)
                {
                    throw new InvalidOperationException($"Ya existe otra categoría con la descripción '{categoriaMovimiento.Descripcion}'");
                }

                return await conexion.UpdateAsync(categoriaMovimiento);
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
                //Obtenemos la conexion a la base de datos
                var conexion = await _servicioBaseDatos.ObtenerConexion();
                return await conexion.DeleteAsync<CategoriaMovimiento>(id);
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
                //Obtenemos la conexion a la base de datos
                var conexion = await _servicioBaseDatos.ObtenerConexion();
                return await conexion.DeleteAsync(categoriaMovimiento);
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
                //Obtenemos la conexion a la base de datos
                var conexion = await _servicioBaseDatos.ObtenerConexion();
                var count = await conexion.Table<CategoriaMovimiento>()
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
                //Obtenemos la conexion a la base de datos
                var conexion = await _servicioBaseDatos.ObtenerConexion();
                var count = await conexion.Table<CategoriaMovimiento>()
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

