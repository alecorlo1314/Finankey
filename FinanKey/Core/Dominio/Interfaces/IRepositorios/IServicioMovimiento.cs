using FinanKey.Dominio.Models;

namespace FinanKey.Dominio.Interfaces
{
    public interface IServicioMovimiento
    {
        Task<List<Movimiento>> ObtenerTodosAsync();
        Task<Movimiento?> ObtenerPorIdAsync(int id);
        Task<List<Movimiento>> ObtenerPorTipoAsync(string tipoMovimiento);
        Task<List<Movimiento>> ObtenerPorCategoriaAsync(int categoriaId);
        Task<List<Movimiento>> ObtenerPorTarjetaAsync(int tarjetaId);
        Task<List<Movimiento>> ObtenerPorComercioAsync(string comercio);
        Task<List<Movimiento>> ObtenerPorFechaAsync(DateTime fechaInicio, DateTime fechaFin);
        Task<List<Movimiento>> ObtenerPorMesAsync(int año, int mes);
        Task<List<Movimiento>> ObtenerPendientesAsync();
        Task<List<Movimiento>> ObtenerPagadosAsync();
        Task<List<Movimiento>> ObtenerRecientesAsync(int limite = 10);
        Task<double> ObtenerTotalPorTipoYFechaAsync(string tipoMovimiento, DateTime fechaInicio, DateTime fechaFin);
        Task<double> ObtenerBalanceDelMesAsync(int año, int mes);
        Task<List<MovimientoResumen>> ObtenerResumenPorCategoriaAsync(DateTime fechaInicio, DateTime fechaFin);
        Task<int> InsertarAsync(Movimiento movimiento);
        Task<int> ActualizarAsync(Movimiento movimiento);
        Task<int> EliminarAsync(int id);
        Task<int> EliminarAsync(Movimiento movimiento);
        Task<int> MarcarComoPagadoAsync(int id);
        Task<int> MarcarComoPendienteAsync(int id);
    }
}
