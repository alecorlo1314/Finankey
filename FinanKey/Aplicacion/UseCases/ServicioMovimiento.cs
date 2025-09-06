using FinanKey.Dominio.Models;
using FinanKey.Dominio.Interfaces;

namespace FinanKey.Aplicacion.UseCases
{
    public class ServicioMovimiento
    {
        //Inyeccion de Dependencias a la interfaz
        private readonly IServicioMovimiento servicioMovimiento;
        public ServicioMovimiento(IServicioMovimiento servicioMovimiento)
        {
            this.servicioMovimiento = servicioMovimiento;
        }
        public Task<int> guardarMovimientoGasto(Movimiento movimientoGasto)
        {
            //retornara 1 si se guardo correctamente sino un 0
            return servicioMovimiento.AgregarMovimientoAsync(movimientoGasto);
        }
    }
}
