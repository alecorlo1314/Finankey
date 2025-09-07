using FinanKey.Dominio.Interfaces;
using FinanKey.Dominio.Models;

namespace FinanKey.Aplicacion.UseCases
{
    public class ServicioTarjeta
    {
        private readonly IServicioTarjeta servicioTarjeta;
        public ServicioTarjeta(IServicioTarjeta servicioTarjeta)
        {
            this.servicioTarjeta = servicioTarjeta;
        }
        //Metodo para obtener tarjetas
        public async Task<List<Tarjeta>> obtenerTarjetas()
        {
           return await servicioTarjeta.ObtenerTarjetasAsync();
        }
    }
}
