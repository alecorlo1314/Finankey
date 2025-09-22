using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FinanKey.Aplicacion.UseCases;
using FinanKey.Dominio.Models;
using FinanKey.Presentacion.View;
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

        #region PROPIEDADES

        [ObservableProperty]
        public Tarjeta? _tarjeta;

        [ObservableProperty]
        public double? _montoTarjeta;

        [ObservableProperty]
        public double? _creditoDisponible;

        [ObservableProperty]
        public bool _isLoading;

        //Propiedades de los Popups
        [ObservableProperty]
        public bool _popupInformacionAbierto;

        [ObservableProperty]
        public string _tituloInformacion = string.Empty;

        [ObservableProperty]
        public string _mensajeInformacion = string.Empty;

        [ObservableProperty]
        public bool _popupEliminacionAbierto;

        [ObservableProperty]
        public bool _popupErrorAbierto;

        [ObservableProperty]
        public string _mensajeError = string.Empty;

        [ObservableProperty]
        public string _tituloError = string.Empty;

        [ObservableProperty]
        public bool _eliminacionTarjeta;

        // Colección de movimientos como propiedad observable
        [ObservableProperty]
        private ObservableCollection<Movimiento> _listaMovimientos = new();

        #endregion Propiedades

        #region PROPIEDADES CALCULADAS

        /// <summary>
        /// Indica si la tarjeta es de tipo crédito
        /// </summary>
        public bool EsTarjetaCredito => Tarjeta?.Tipo == "Credito";

        /// <summary>
        /// Indica si la tarjeta es de tipo débito
        /// </summary>
        public bool EsTarjetaDebito => Tarjeta?.Tipo == "Debito";

        /// <summary>
        /// Texto que describe el tipo de monto mostrado
        /// </summary>
        public string TipoMontoTexto => EsTarjetaCredito ? "Límite de Crédito" : "Saldo Disponible";

        /// <summary>
        /// Texto del crédito disponible formateado
        /// </summary>
        public string CreditoDisponibleTexto => EsTarjetaCredito
            ? $"Disponible: ${CreditoDisponible:N2}"
            : string.Empty;
        #endregion

        #region CONSTRUCTOR
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="servicioDetallesTarjeta"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public ViewModelDetalleTarjeta(ServicioDetallesTarjeta servicioDetallesTarjeta)
        {
            _servicioDetallesTarjeta = servicioDetallesTarjeta ?? throw new ArgumentNullException(nameof(servicioDetallesTarjeta));
        }
        #endregion

        #region Metodo para obtener los detalles de la tarjeta con parametros desde otro viewModel

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
                // Cargar los movimientos de la tarjeta
                CargarMovimientosAsync();
            }
            catch (Exception ex)
            {
                Shell.Current.DisplayAlert("Error", $"Ocurrio un error en el metodo ApplyQueryAttributes: {ex}", "Cerrar");
            }
        }

        #endregion

        #region MÉTODOS PRIVADOS

        /// <summary>
        /// Inicializa los datos de la tarjeta y carga los movimientos
        /// </summary>
        private async Task InicializarTarjetaAsync()
        {
            // Configurar el monto según el tipo de tarjeta
            MontoTarjeta = EsTarjetaCredito ? (Tarjeta?.LimiteCredito ?? 0) : (Tarjeta?.MontoInicial ?? 0);

            if (EsTarjetaCredito)
            {
                CalcularCreditoDisponible();
            }
            // Cargar movimientos
            await CargarMovimientosAsync();
        }
        #endregion

        /// <summary>
        /// Calcula el crédito disponible para tarjetas de crédito
        /// </summary>
        private void CalcularCreditoDisponible()
        {
            if (EsTarjetaCredito && Tarjeta?.LimiteCredito.HasValue == true)
            {
                var creditoUsado = Tarjeta.CreditoUsado ?? 0;
                CreditoDisponible = MontoTarjeta - creditoUsado;
            }
        }
        /// <summary>
        /// Carga los movimientos de la tarjeta desde la base de datos
        /// </summary>
        private async Task CargarMovimientosAsync()
        {
            try
            {
                //Si tarjeta es nula no se hace la carga de movimientos
                if (Tarjeta == null) return;
                //cargando movimientos
                IsLoading = true;
                //realizar la consulta a la base de datos
                var movimientos = await _servicioDetallesTarjeta.ObtenerMovimientosPorTarjeta(Tarjeta.Id);
                //si no hay movimientos asociados a la tarjeta
                if (!movimientos.Any()) return;
                // Limpiar y cargar nuevos movimientos
                ListaMovimientos.Clear();
                //si hay movimientos asociados a la tarjeta se guardan en la lista de movimientos
                ListaMovimientos = new ObservableCollection<Movimiento>(movimientos);
            }
            catch (Exception ex)
            {
                MostrarError("Error cargando movimientos", ex.Message);
            }
            finally
            {
                IsLoading = false;
            }
        }

        #region REGION DE COMANDOS

        /// <summary>
        /// Elimina una tarjeta de la base de datos
        /// </summary>
        /// <returns></returns>
        [RelayCommand]
        private void EliminarTarjeta()
        {
            PopupEliminacionAbierto = true;
        }
        /// <summary>
        /// Confirma la eliminacion de la tarjeta de la base de datos
        /// </summary>
        /// <returns></returns>
        [RelayCommand]
        private async Task ConfirmarEliminacionTarjeta()
        {
            try
            {
                // Verificar si la tarjeta es nula
                if (Tarjeta == null) return;
                //mostrar cargando
                IsLoading = true;
                //Verificar que la tarjeta no tenga movimientos asociados
                if (ListaMovimientos.Any())
                {
                    PopupEliminacionAbierto = false;
                    MostrarError("Error al eliminar Tarjeta", "No se pudo eliminar una tarjeta que tiene transacciones asociadas");
                    return;
                }
                //Si la tarjeta se pudo eliminar devulve la cantidad de filas afectadas en la base de datos
                var resultado = await _servicioDetallesTarjeta.EliminarTarjetaAsync(Tarjeta.Id);
                //si la cantidad de filas afectadas es mayor a 0 se elimino correctamente
                if (resultado > 0)
                {
                    MostrarInformacion("Informacion", "Tarjeta eliminada correctamente");
                    // Esperar un momento para que el usuario vea el mensaje
                    await Task.Delay(1500);
                    // Navegar de regreso
                    await Shell.Current.GoToAsync("..", true);
                }
                else
                {
                    MostrarError("Error", "No se pudo eliminar la tarjeta");
                }
            }
            catch (Exception ex)
            {
                MostrarError("Error eliminando tarjeta", ex.Message);
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private void CancelarEliminacionTarjeta()
        {
            EliminacionTarjeta = false;
            PopupEliminacionAbierto = false;
        }

        [RelayCommand]
        private void MostrarInformacionTarjeta(Tarjeta tarjeta)
        {
            MensajeInformacion = tarjeta.Descripcion;
            PopupInformacionAbierto = true;
        }

        [RelayCommand]
        private void CerrarPopupError()
        {
            PopupErrorAbierto = false;
            TituloError = string.Empty;
            MensajeError = string.Empty;
        }

        #endregion REGION DE COMANDOS

        #region Helpers

        private void MostrarError(string titulo, string mensaje)
        {
            TituloError = titulo;
            MensajeError = mensaje;
            PopupErrorAbierto = true;
        }

        private void MostrarInformacion(string titulo, string mensaje)
        {
            TituloInformacion = titulo;
            MensajeInformacion = mensaje;
            PopupInformacionAbierto = true;
        }

        #endregion Helpers
    }
}