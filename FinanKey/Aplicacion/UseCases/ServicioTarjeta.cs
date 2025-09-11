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
        public async Task<List<Tarjeta>> ObtenerTodosAsync()
        {
           return await servicioTarjeta.ObtenerTodosAsync();
        }
        public async Task<int> InsertarAsync(Tarjeta tarjeta)
        {
            return await servicioTarjeta.InsertarAsync(tarjeta);
        }
        public async Task<int> EliminarAsync(int idTarjeta)
        {
            return await servicioTarjeta.EliminarAsync(idTarjeta);
        }
    }
}
