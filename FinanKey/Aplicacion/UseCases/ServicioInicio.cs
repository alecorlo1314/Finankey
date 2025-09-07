
using FinanKey.Dominio.Interfaces;
using FinanKey.Dominio.Models;

namespace FinanKey.Aplicacion.UseCases
{
    public class ServicioInicio
    {
        private readonly IServicioMovimiento servicioMovimiento;
        private readonly IServicioTarjeta servicioTarjeta;
        public ServicioInicio(IServicioMovimiento servicioMovimiento, IServicioTarjeta servicioTarjeta)
        {
            this.servicioMovimiento = servicioMovimiento;
            this.servicioTarjeta = servicioTarjeta;
        }
        public async Task<List<Tarjeta>> ObtenerTarjetasAsync()
        {
            return await servicioTarjeta.ObtenerTarjetasAsync();
        }
        public async Task<List<Movimiento>> ObtenerMovimientosAsync()
        {
            return await servicioMovimiento.ObtenerMovimientosAsync();
        }
    }
}
