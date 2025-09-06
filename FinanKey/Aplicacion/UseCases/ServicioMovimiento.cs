using FinanKey.Dominio.Models;
using FinanKey.Dominio.Interfaces;

namespace FinanKey.Aplicacion.UseCases
{
    public class ServicioMovimiento
    {
        //Inyeccion de Dependencias a la interfaz
        private readonly IServicioMovimiento _servicioMovimiento;
        private readonly IServicioTarjeta _servicioTarjeta;
        public ServicioMovimiento(IServicioMovimiento servicioMovimiento, IServicioTarjeta servicioTarjeta)
        {
            _servicioMovimiento = servicioMovimiento;
            _servicioTarjeta = servicioTarjeta;
        }
        public Task<int> guardarMovimientoGasto(Movimiento movimientoGasto)
        {
            //retornara 1 si se guardo correctamente sino un 0
            return _servicioMovimiento.AgregarMovimientoAsync(movimientoGasto);
        }
        public async Task<List<Tarjeta>> obtenerTarjetas()
        {
           return await _servicioTarjeta.ObtenerTarjetasAsync();
        }
    }
}
