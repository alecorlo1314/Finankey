using FinanKey.Dominio.Models;
using FinanKey.Dominio.Interfaces;

namespace FinanKey.Aplicacion.UseCases
{
    public class ServicioCategoriaMovimiento
    {
        private readonly IServicioCategoriaMovimiento _servicioCategoriaMovimiento;
        public ServicioCategoriaMovimiento(IServicioCategoriaMovimiento servicioCategoriaMovimiento)
        {
            _servicioCategoriaMovimiento = servicioCategoriaMovimiento;
        }
        public async Task<int> guardarCategoriaMovimiento(CategoriaMovimiento nuevaCategoriaMovimiento)
        {
            return await _servicioCategoriaMovimiento.InsertarAsync(nuevaCategoriaMovimiento);
        }
        public async Task<List<CategoriaMovimiento>> ObtenerPorTipoMovimientoAsync(string tipoMovimiento)
        {
            return await _servicioCategoriaMovimiento.ObtenerPorTipoMovimientoAsync(tipoMovimiento);
        }
        public async Task<List<CategoriaMovimiento>> ObtenerTodasAsync()
        {
            return await _servicioCategoriaMovimiento.ObtenerTodosAsync();
        }
        public async Task<int> EliminarCategoriaMovimiento(int idCategoriaMovimiento)
        {
            return await _servicioCategoriaMovimiento.EliminarAsync(idCategoriaMovimiento);
        }
    }
}
