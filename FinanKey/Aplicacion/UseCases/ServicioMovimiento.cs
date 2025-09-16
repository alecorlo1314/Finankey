using FinanKey.Dominio.Models;
using FinanKey.Dominio.Interfaces;

namespace FinanKey.Aplicacion.UseCases
{
    public class ServicioMovimiento
    {
        //Inyeccion de Dependencias a la interfaz
        private readonly IServicioMovimiento _servicioMovimiento;
        private readonly IServicioTarjeta _servicioTarjeta;
        private readonly IServicioCategoriaMovimiento _servicioCategoriaMovimiento;
        public ServicioMovimiento(IServicioMovimiento servicioMovimiento, 
            IServicioTarjeta servicioTarjeta, 
            IServicioCategoriaMovimiento servicioCategoriaMovimiento)
        {
            _servicioMovimiento = servicioMovimiento;
            _servicioTarjeta = servicioTarjeta;
            _servicioCategoriaMovimiento = servicioCategoriaMovimiento;
        }
        /// <summary>
        /// Guarda un movimiento de gasto
        /// </summary>
        /// <param name="movimientoGasto"></param>
        /// <returns></returns>
        public Task<int> guardarMovimiento(Movimiento movimientoGasto)
        {
            return _servicioMovimiento.InsertarAsync(movimientoGasto);
        }
        /// <summary>
        /// Obtiene todas las tarjetas registradas en las bases de datos
        /// </summary>
        /// <returns></returns>
        public async Task<List<Tarjeta>?> obtenerTarjetas()
        {
           return await _servicioTarjeta.ObtenerTodosAsync();
        }
        /// <summary>
        /// Obtiene todas las categorias registradas en las base de datos
        /// </summary>
        /// <returns></returns>
        public async Task<List<CategoriaMovimiento?>> ObtenerCategoriasMovimientoAsync()
        {
            return await _servicioCategoriaMovimiento.ObtenerTodosAsync();
        }
        /// <summary>
        /// Obtiene todas las categorias de tipo Gasto
        /// </summary>
        /// <returns></returns>
        public async Task<List<CategoriaMovimiento?>> ObtenerCategoriasTipoGastosAsync()
        {
            return await _servicioCategoriaMovimiento.ObtenerCategoriasTipoGastosAsync();
        }
        /// <summary>
        /// Obtiene todas las categorias de tipo Ingresos
        /// </summary>
        /// <returns></returns>
        public async Task<List<CategoriaMovimiento?>> ObtenerCategoriasTipoIngresosAsync()
        {
            return await _servicioCategoriaMovimiento.ObtenerCategoriasTipoIngresosAsync();
        }
    }
}
