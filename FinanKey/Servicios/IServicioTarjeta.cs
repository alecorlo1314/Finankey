using FinanKey.Models;
namespace FinanKey.Servicios
{
    public interface IServicioTarjeta
    {
        public Task<int> AgregarAsync(Tarjeta Nuevatarjeta);
        public Task ActualizarAsync(Tarjeta TarjetaActualizada);
        public Task EliminarAsync(int idTarjeta);
        public Task<List<Tarjeta>> ObtenerTarjetasAsync();
        public Task<Tarjeta?> ObtenerTarjetaPorIdAsync(int idTarjeta);
    }
}
