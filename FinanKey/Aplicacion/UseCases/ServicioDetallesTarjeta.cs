using FinanKey.Dominio.Interfaces;
using FinanKey.Dominio.Models;

namespace FinanKey.Aplicacion.UseCases
{
    public class ServicioDetallesTarjeta
    {
        /// <summary>
        /// Se utiliza los metodos de la interface IServicioTarjeta
        /// como inyeccion de dependencias
        /// </summary>
        private readonly IServicioTarjeta _servicioTarjeta;
        /// <summary>
        /// Se utiliza los metodos de la interface IServicioMovimiento
        /// como inyeccion de dependencias
        /// </summary>
        private readonly IServicioMovimiento _servicioMovimiento;
        public ServicioDetallesTarjeta(IServicioTarjeta servicioTarjeta, IServicioMovimiento servicioMovimiento)
        {
            //Se inyecta la interface
            _servicioTarjeta = servicioTarjeta;
            _servicioMovimiento = servicioMovimiento;
        }
        /// <summary>
        /// Obtiene los movimientos asociados a una tarjeta por su id
        /// </summary>
        /// <param name="tarjetaId"></param>
        /// <returns></returns>
        public async Task<List<Movimiento>> ObtenerMovimientosPorTarjeta(int tarjetaId)
        {
            //Obtiene los movimientos asociados a una tarjeta
           return await _servicioMovimiento.ObtenerPorTarjetaAsync(tarjetaId);
        }
    }
}