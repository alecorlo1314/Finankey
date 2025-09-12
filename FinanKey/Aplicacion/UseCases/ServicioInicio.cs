
using FinanKey.Dominio.Interfaces;
using FinanKey.Dominio.Models;

namespace FinanKey.Aplicacion.UseCases
{
    public class ServicioInicio
    {
        private readonly IServicioMovimiento servicioMovimiento;
        private readonly ServicioTarjeta servicioTarjeta;
        public ServicioInicio(IServicioMovimiento servicioMovimiento, ServicioTarjeta servicioTarjeta)
        {
            this.servicioMovimiento = servicioMovimiento;
            this.servicioTarjeta = servicioTarjeta;
        }
        public async Task<List<Tarjeta>> ObtenerTarjetasAsync()
        {
            return await servicioTarjeta.ObtenerTodosAsync();
        }
        public async Task<List<Movimiento>> ObtenerMovimientosAsync()
        {
            return await servicioMovimiento.ObtenerMovimientosAsync();
        }
    }
}
