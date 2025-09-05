using FinanKey.Dominio.Models;

namespace FinanKey.Dominio.Interfaces
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
