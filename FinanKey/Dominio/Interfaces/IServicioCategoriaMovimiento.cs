
using FinanKey.Dominio.Models;

namespace FinanKey.Dominio.Interfaces
{
    internal interface IServicioCategoriaMovimiento
    {
        Task<List<CategoriaMovimiento>> ObtenerTodosAsync();
        Task<CategoriaMovimiento?> ObtenerPorIdAsync(int id);
        Task<List<CategoriaMovimiento>> ObtenerPorTipoMovimientoAsync(string tipoMovimiento);
        Task<CategoriaMovimiento?> ObtenerPorDescripcionAsync(string descripcion);
        Task<int> InsertarAsync(CategoriaMovimiento categoriaMovimiento);
        Task<int> ActualizarAsync(CategoriaMovimiento categoriaMovimiento);
        Task<int> EliminarAsync(int id);
        Task<int> EliminarAsync(CategoriaMovimiento categoriaMovimiento);
        Task<bool> ExisteDescripcionAsync(string descripcion);
        Task<bool> ExisteDescripcionAsync(string descripcion, int idExcluir);
    }
}
