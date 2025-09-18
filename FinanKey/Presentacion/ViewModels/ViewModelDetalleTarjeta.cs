
using CommunityToolkit.Mvvm.ComponentModel;
using FinanKey.Dominio.Models;

namespace FinanKey.Presentacion.ViewModels
{
    /// <summary>
    /// ViewModel para la pagina de detalle de la tarjeta
    /// Esta hereda de la clase ObservableObject para notificar cambios en las propiedades de una forma mas sencilla
    /// Tambien hereda de la interface IQueryAttributable para recibir parametros de navegacion
    /// </summary>
    public partial class ViewModelDetalleTarjeta : ObservableObject, IQueryAttributable
    {
        [ObservableProperty]
        public Tarjeta tarjeta;

        #region Metodo de la Interface IQueryAttributable
        /// <summary>
        /// Metodo para recibir parametros de navegacion desde el viewModelInicio
        /// Este metodo es llamado automaticamente cuando se navega a esta pagina
        /// </summary>
        /// <param name="query"></param>
        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            Tarjeta = query["Tarjeta"] as Tarjeta;
        }
        #endregion
    }
}
