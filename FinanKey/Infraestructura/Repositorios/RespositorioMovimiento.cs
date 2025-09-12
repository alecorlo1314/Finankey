using FinanKey.Dominio.Interfaces;
using FinanKey.Dominio.Models;

namespace FinanKey.Infraestructura.Repositorios
{
    public class RespositorioMovimiento : IServicioMovimiento
    {
        private readonly RepositorioBaseDatos _repositorioBaseDatos;

        //Inyeccion de dependencias para el servicio de base de datos
        public RespositorioMovimiento(RepositorioBaseDatos repositorioBaseDatos)
        {
            _repositorioBaseDatos = repositorioBaseDatos;
        }

        /// <summary>
        /// Obtiene todos los movimientos ordenados por fecha más reciente
        /// </summary>
        public async Task<List<Movimiento>> ObtenerTodosAsync()
        {
            try
            {
                var conexion = await _repositorioBaseDatos.ObtenerConexion();
                return await conexion.Table<Movimiento>()
                                   .OrderByDescending(m => m.FechaMovimiento)
                                   .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener todos los movimientos: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Obtiene un movimiento por su ID
        /// </summary>
        public async Task<Movimiento?> ObtenerPorIdAsync(int id)
        {
            try
            {
                var conexion = await _repositorioBaseDatos.ObtenerConexion();
                return await conexion.Table<Movimiento>()
                                   .Where(m => m.Id == id)
                                   .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener movimiento por ID {id}: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Obtiene movimientos por tipo (Ingreso/Gasto)
        /// </summary>
        public async Task<List<Movimiento>> ObtenerPorTipoAsync(string tipoMovimiento)
        {
            try
            {
                var conexion = await _repositorioBaseDatos.ObtenerConexion();
                return await conexion.Table<Movimiento>()
                                   .Where(m => m.TipoMovimiento == tipoMovimiento)
                                   .OrderByDescending(m => m.FechaMovimiento)
                                   .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener movimientos por tipo {tipoMovimiento}: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Obtiene movimientos por categoría
        /// </summary>
        public async Task<List<Movimiento>> ObtenerPorCategoriaAsync(int categoriaId)
        {
            try
            {
                var conexion = await _repositorioBaseDatos.ObtenerConexion();
                return await conexion.Table<Movimiento>()
                                   .Where(m => m.CategoriaId == categoriaId)
                                   .OrderByDescending(m => m.FechaMovimiento)
                                   .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener movimientos por categoría {categoriaId}: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Obtiene movimientos por tarjeta
        /// </summary>
        public async Task<List<Movimiento>> ObtenerPorTarjetaAsync(int tarjetaId)
        {
            try
            {
                var conexion = await _repositorioBaseDatos.ObtenerConexion();
                return await conexion.Table<Movimiento>()
                                   .Where(m => m.TarjetaId == tarjetaId)
                                   .OrderByDescending(m => m.FechaMovimiento)
                                   .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener movimientos por tarjeta {tarjetaId}: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Obtiene movimientos por comercio
        /// </summary>
        public async Task<List<Movimiento>> ObtenerPorComercioAsync(string comercio)
        {
            try
            {
                var conexion = await _repositorioBaseDatos.ObtenerConexion();
                return await conexion.Table<Movimiento>()
                                   .Where(m => m.Comercio == comercio)
                                   .OrderByDescending(m => m.FechaMovimiento)
                                   .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener movimientos por comercio {comercio}: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Obtiene movimientos en un rango de fechas
        /// </summary>
        public async Task<List<Movimiento>> ObtenerPorFechaAsync(DateTime fechaInicio, DateTime fechaFin)
        {
            try
            {
                var conexion = await _repositorioBaseDatos.ObtenerConexion();
                return await conexion.Table<Movimiento>()
                                   .Where(m => m.FechaMovimiento >= fechaInicio && m.FechaMovimiento <= fechaFin)
                                   .OrderByDescending(m => m.FechaMovimiento)
                                   .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener movimientos por fecha: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Obtiene movimientos de un mes específico
        /// </summary>
        public async Task<List<Movimiento>> ObtenerPorMesAsync(int año, int mes)
        {
            try
            {
                var fechaInicio = new DateTime(año, mes, 1);
                var fechaFin = fechaInicio.AddMonths(1).AddDays(-1);
                return await ObtenerPorFechaAsync(fechaInicio, fechaFin);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener movimientos del mes {mes}/{año}: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Obtiene movimientos pendientes (no pagados)
        /// </summary>
        public async Task<List<Movimiento>> ObtenerPendientesAsync()
        {
            try
            {
                var conexion = await _repositorioBaseDatos.ObtenerConexion();
                return await conexion.Table<Movimiento>()
                                   .Where(m => m.EsPagado == false || m.EsPagado == null)
                                   .OrderByDescending(m => m.FechaMovimiento)
                                   .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener movimientos pendientes: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Obtiene movimientos pagados
        /// </summary>
        public async Task<List<Movimiento>> ObtenerPagadosAsync()
        {
            try
            {
                var conexion = await _repositorioBaseDatos.ObtenerConexion();
                return await conexion.Table<Movimiento>()
                                   .Where(m => m.EsPagado == true)
                                   .OrderByDescending(m => m.FechaMovimiento)
                                   .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener movimientos pagados: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Obtiene los movimientos más recientes
        /// </summary>
        public async Task<List<Movimiento>> ObtenerRecientesAsync(int limite = 10)
        {
            try
            {
                var conexion = await _repositorioBaseDatos.ObtenerConexion();
                return await conexion.Table<Movimiento>()
                                   .OrderByDescending(m => m.FechaMovimiento)
                                   .Take(limite)
                                   .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener movimientos recientes: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Obtiene el total de un tipo de movimiento en un rango de fechas
        /// </summary>
        public async Task<double> ObtenerTotalPorTipoYFechaAsync(string tipoMovimiento, DateTime fechaInicio, DateTime fechaFin)
        {
            try
            {
                var conexion = await _repositorioBaseDatos.ObtenerConexion();
                var movimientos = await conexion.Table<Movimiento>()
                                              .Where(m => m.TipoMovimiento == tipoMovimiento &&
                                                         m.FechaMovimiento >= fechaInicio &&
                                                         m.FechaMovimiento <= fechaFin)
                                              .ToListAsync();

                return movimientos.Sum(m => m.Monto);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al calcular total por tipo {tipoMovimiento}: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Obtiene el balance (ingresos - gastos) de un mes
        /// </summary>
        public async Task<double> ObtenerBalanceDelMesAsync(int año, int mes)
        {
            try
            {
                var fechaInicio = new DateTime(año, mes, 1);
                var fechaFin = fechaInicio.AddMonths(1).AddDays(-1);

                var totalIngresos = await ObtenerTotalPorTipoYFechaAsync("Ingreso", fechaInicio, fechaFin);
                var totalGastos = await ObtenerTotalPorTipoYFechaAsync("Gasto", fechaInicio, fechaFin);

                return totalIngresos - totalGastos;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al calcular balance del mes {mes}/{año}: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Obtiene resumen de movimientos agrupados por categoría
        /// </summary>
        public async Task<List<MovimientoResumen>> ObtenerResumenPorCategoriaAsync(DateTime fechaInicio, DateTime fechaFin)
        {
            try
            {
                var conexion = await _repositorioBaseDatos.ObtenerConexion();

                // Query SQL para obtener resumen por categoría
                var query = @"
                    SELECT 
                        m.CategoriaId,
                        m.TipoMovimiento,
                        COUNT(*) as Cantidad,
                        SUM(m.Monto) as Total,
                        AVG(m.Monto) as Promedio
                    FROM Movimiento m 
                    WHERE m.FechaMovimiento BETWEEN ? AND ?
                    GROUP BY m.CategoriaId, m.TipoMovimiento
                    ORDER BY Total DESC";

                var resultado = await conexion.QueryAsync<MovimientoResumen>(query, fechaInicio, fechaFin);
                return resultado;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener resumen por categoría: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Inserta un nuevo movimiento
        /// </summary>
        public async Task<int> InsertarAsync(Movimiento movimiento)
        {
            try
            {
                // Validaciones básicas
                ValidarMovimiento(movimiento);

                var conexion = await _repositorioBaseDatos.ObtenerConexion();
                movimiento.FechaCreacion = DateTime.Now;

                return await conexion.InsertAsync(movimiento);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al insertar movimiento: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Actualiza un movimiento existente
        /// </summary>
        public async Task<int> ActualizarAsync(Movimiento movimiento)
        {
            try
            {
                ValidarMovimiento(movimiento);

                var conexion = await _repositorioBaseDatos.ObtenerConexion();
                return await conexion.UpdateAsync(movimiento);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al actualizar movimiento: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Elimina un movimiento por ID
        /// </summary>
        public async Task<int> EliminarAsync(int id)
        {
            try
            {
                var conexion = await _repositorioBaseDatos.ObtenerConexion();
                return await conexion.DeleteAsync<Movimiento>(id);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al eliminar movimiento con ID {id}: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Elimina un movimiento por objeto
        /// </summary>
        public async Task<int> EliminarAsync(Movimiento movimiento)
        {
            try
            {
                var conexion = await _repositorioBaseDatos.ObtenerConexion();
                return await conexion.DeleteAsync(movimiento);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al eliminar movimiento: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Marca un movimiento como pagado
        /// </summary>
        public async Task<int> MarcarComoPagadoAsync(int id)
        {
            try
            {
                var conexion = await _repositorioBaseDatos.ObtenerConexion();
                var movimiento = await ObtenerPorIdAsync(id);

                if (movimiento == null)
                    throw new InvalidOperationException($"No se encontró el movimiento con ID {id}");

                movimiento.EsPagado = true;
                return await conexion.UpdateAsync(movimiento);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al marcar movimiento como pagado: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Marca un movimiento como pendiente
        /// </summary>
        public async Task<int> MarcarComoPendienteAsync(int id)
        {
            try
            {
                var conexion = await _repositorioBaseDatos.ObtenerConexion();
                var movimiento = await ObtenerPorIdAsync(id);

                if (movimiento == null)
                    throw new InvalidOperationException($"No se encontró el movimiento con ID {id}");

                movimiento.EsPagado = false;
                return await conexion.UpdateAsync(movimiento);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al marcar movimiento como pendiente: {ex.Message}", ex);
            }
        }
        /// <summary>
        /// Valida que el movimiento tenga los datos requeridos
        /// </summary>
        private static void ValidarMovimiento(Movimiento movimiento)
        {
            if (movimiento == null)
                throw new ArgumentNullException(nameof(movimiento));

            if (string.IsNullOrWhiteSpace(movimiento.TipoMovimiento))
                throw new ArgumentException("El tipo de movimiento es requerido");

            if (movimiento.Monto <= 0)
                throw new ArgumentException("El monto debe ser mayor a 0");

            if (movimiento.CategoriaId <= 0)
                throw new ArgumentException("La categoría es requerida");

            if (movimiento.TarjetaId <= 0)
                throw new ArgumentException("La tarjeta es requerida");

            if (string.IsNullOrWhiteSpace(movimiento.Comercio))
                throw new ArgumentException("El comercio es requerido");
        }
    }

    /// <summary>
    /// Extensiones útiles para análisis de movimientos
    /// </summary>
    public static class MovimientoExtensions
    {
        public static bool EsDelMesActual(this Movimiento movimiento)
        {
            var hoy = DateTime.Now;
            return movimiento.FechaMovimiento.Year == hoy.Year &&
                   movimiento.FechaMovimiento.Month == hoy.Month;
        }

        public static bool EsDelDiaActual(this Movimiento movimiento)
        {
            return movimiento.FechaMovimiento.Date == DateTime.Now.Date;
        }

        public static string ObtenerEstadoPago(this Movimiento movimiento)
        {
            return movimiento.EsPagado == true ? "Pagado" : "Pendiente";
        }

        public static string ObtenerFechaAmigable(this Movimiento movimiento)
        {
            var diferencia = DateTime.Now - movimiento.FechaMovimiento;

            return diferencia.TotalDays switch
            {
                0 => "Hoy",
                1 => "Ayer",
                < 7 => $"Hace {(int)diferencia.TotalDays} días",
                < 30 => $"Hace {(int)(diferencia.TotalDays / 7)} semanas",
                _ => movimiento.FechaMovimiento.ToString("dd MMM yyyy")
            };
        }
    }
}