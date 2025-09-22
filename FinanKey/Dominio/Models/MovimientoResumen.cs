
namespace FinanKey.Dominio.Models
{
    /// <summary>
    /// Clase para resúmenes de movimientos por categoría
    /// </summary>
    public class MovimientoResumen
    {
        public int CategoriaId { get; set; }
        public string TipoMovimiento { get; set; } = string.Empty;
        public int Cantidad { get; set; }
        public double Total { get; set; }
        public double Promedio { get; set; }
    }

}
