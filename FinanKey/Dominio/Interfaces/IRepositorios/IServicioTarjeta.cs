using FinanKey.Dominio.Models;

namespace FinanKey.Dominio.Interfaces
{
    public interface IServicioTarjeta
    {
        Task<List<Tarjeta>> ObtenerTodosAsync();
        Task<Tarjeta?> ObtenerPorIdAsync(int id);
        Task<List<Tarjeta>> ObtenerPorTipoAsync(string tipo);
        Task<List<Tarjeta>> ObtenerPorBancoAsync(string banco);
        Task<Tarjeta?> ObtenerPorUltimos4DigitosAsync(string ultimos4Digitos);
        Task<List<Tarjeta>> ObtenerTarjetasVencenProntoAsync(int diasAnticipacion = 30);
        Task<List<Tarjeta>> ObtenerTarjetasVencidasAsync();
        Task<int> InsertarAsync(Tarjeta tarjeta);
        Task<int> ActualizarAsync(Tarjeta tarjeta);
        Task<int> EliminarAsync(int id);
        Task<int> EliminarAsync(Tarjeta tarjeta);
        Task<bool> ExisteTarjetaConUltimos4DigitosAsync(string ultimos4Digitos);
        Task<bool> ExisteTarjetaConUltimos4DigitosAsync(string ultimos4Digitos, int idExcluir);
    }
}
