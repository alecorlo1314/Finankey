using CommunityToolkit.Mvvm.ComponentModel;
using FinanKey.Aplicacion.UseCases;
using FinanKey.Dominio.Models;
using System.Collections.ObjectModel;

namespace FinanKey.Presentacion.ViewModels
{
    /// <summary>
    /// ViewModel para la pagina de detalle de la tarjeta
    /// Esta hereda de la clase ObservableObject para notificar cambios en las propiedades de una forma mas sencilla
    /// Tambien hereda de la interface IQueryAttributable para recibir parametros de navegacion
    /// </summary>
    public partial class ViewModelDetalleTarjeta : ObservableObject, IQueryAttributable
    {
        #region DEPENDENCIAS

        /// <summary>
        /// Servicio para obtener los detalles de la tarjeta
        /// </summary>
        private readonly ServicioDetallesTarjeta _servicioDetallesTarjeta;

        #endregion DEPENDENCIAS

        #region Propiedades

        [ObservableProperty]
        public Tarjeta? tarjeta;

        [ObservableProperty]
        public double? _montoTarjeta;

        [ObservableProperty]
        public double? _creditoDisponible;

        public ObservableCollection<Movimiento?> ListaMovimientos = [];

        #endregion Propiedades

        public ViewModelDetalleTarjeta(ServicioDetallesTarjeta servicioDetallesTarjeta)
        {
            //Inicializar las dependencias
            _servicioDetallesTarjeta = servicioDetallesTarjeta ?? throw new ArgumentNullException(nameof(servicioDetallesTarjeta));
        }

        #region Metodo de la Interface IQueryAttributable

        /// <summary>
        /// Metodo para recibir parametros de navegacion desde el viewModelInicio
        /// Este metodo es llamado automaticamente cuando se navega a esta pagina
        /// </summary>
        /// <param name="query"></param>
        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            try
            {
                Tarjeta = query["Tarjeta"] as Tarjeta;

                if (Tarjeta?.MontoInicial != null)
                {
                    MontoTarjeta = Tarjeta?.MontoInicial;
                }
                else
                {
                    MontoTarjeta = Tarjeta?.LimiteCredito;
                    CalcularCreditoDisponible();
                }
            }
            catch (Exception ex)
            {
                Shell.Current.DisplayAlert("Error", $"Ocurrio un error en el metodo ApplyQueryAttributes: {ex}", "Cerrar");
            }
        }
        /// <summary>
        /// Metodo para calcular el credito disponible de la tarjeta 
        /// la operacion es credito disponible = monto tarjeta - credito usado cada ves que se realiza un movimiento con la tarjeta
        /// </summary>
        private void CalcularCreditoDisponible()
        {
            if (MontoTarjeta != null && Tarjeta?.LimiteCredito != null)
            {
                CreditoDisponible = MontoTarjeta - Tarjeta?.CreditoUsado;
            }
        }

        #endregion
    }
}