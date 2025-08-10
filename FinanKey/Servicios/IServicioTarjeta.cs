using FinanKey.Models;
namespace FinanKey.Servicios
{
    public interface IServicioTarjeta
    {
        public Task<int> AgregarAsync(Tarjeta Nuevatarjeta);
        public Task<int> ActualizarAsync(Tarjeta TarjetaActualizada);
        public Task<int> EliminarAsync(int idTarjeta);
        public Task<List<Tarjeta>> ObtenerTarjetasAsync();
        public Task<Tarjeta?> ObtenerTarjetaPorIdAsync(int idTarjeta);
    }
}
