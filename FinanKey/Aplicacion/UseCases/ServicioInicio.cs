
using FinanKey.Dominio.Interfaces;

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
    }
}
