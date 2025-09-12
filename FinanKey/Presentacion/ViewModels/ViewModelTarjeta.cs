using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FinanKey.Aplicacion.UseCases;
using FinanKey.Dominio.Models;

using System.Collections.ObjectModel;

namespace FinanKey.Presentacion.ViewModels
{
    public partial class ViewModelTarjeta : ObservableObject
    {
        #region DEPENDENCIAS

        private readonly ServicioTarjeta _servicioTarjeta;

        #endregion DEPENDENCIAS

        #region PROPIEDADES DE ESTADO

        [ObservableProperty]
        private bool _isLoading;

        [ObservableProperty]
        private bool _isGuardando;

        [ObservableProperty]
        private bool _hasError;

        [ObservableProperty]
        private string _mensajeError = string.Empty;

        #endregion PROPIEDADES DE ESTADO

        #region PROPIEDADES DE FORMULARIO

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(AgregarTarjetaCommand))]
        private string _nombreTarjeta = string.Empty;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(AgregarTarjetaCommand))]
        private string _ultimosCuatroDigitos = string.Empty;

        [ObservableProperty]
        private string _banco = string.Empty;

        [ObservableProperty]
        private string _vencimiento = string.Empty;

        [ObservableProperty]
        private string _limiteCredito = string.Empty; //Solo para tarjetas de crédito

        [ObservableProperty]
        private string _montoInicial = string.Empty; //Solo para tarjetas de débito

        [ObservableProperty]
        private string _descripcion = string.Empty;

        // Estados de UI
        [ObservableProperty]
        private bool _esVisibleMonto = false;

        [ObservableProperty]
        private bool _esVisibleLimiteCredito = true;

        [ObservableProperty]
        public string? _mensajeInformacion; //mensaje que se muestra en el Popup de informacion

        [ObservableProperty]
        public bool _popupInformacionAbierto;// Popup se abrira cuando este en true

        // Propiedades calculadas para el tipo de tarjeta
        public string TipoTarjetaSeleccionado => EsVisibleMonto ? "Débito" : "Crédito";

        public bool EsTarjetaCredito => !EsVisibleMonto;
        public bool EsTarjetaDebito => EsVisibleMonto;

        #endregion PROPIEDADES DE FORMULARIO

        #region PROPIEDADES DE DISEÑO

        [ObservableProperty]
        private string _logoTarjeta = "icono_visa.svg";

        [ObservableProperty]
        private string _categoria = "Visa";

        [ObservableProperty]
        private string _linearColor1 = "#3E298F";

        [ObservableProperty]
        private string _linearColor2 = "#836EDB";

        #endregion PROPIEDADES DE DISEÑO

        #region COLECCIONES

        [ObservableProperty]
        private ObservableCollection<Tarjeta> _listaTarjetas = new();

        public ObservableCollection<OpcionTarjeta> ListaLogoTarjeta { get; private set; } = new();

        #endregion COLECCIONES

        #region PROPIEDADES DE VALIDACIÓN

        public bool NombreTarjetaEsValido => !string.IsNullOrWhiteSpace(NombreTarjeta) && NombreTarjeta.Length >= 2 && NombreTarjeta.Length <= 100;
        public bool UltimosCuatroDigitosEsValido => UltimosCuatroDigitos?.Length == 4;
        public bool VencimientoEsValido => string.IsNullOrEmpty(Vencimiento) || EsFormatoVencimientoValido(Vencimiento);

        #endregion PROPIEDADES DE VALIDACIÓN

        #region CONSTRUCTOR

        public ViewModelTarjeta(ServicioTarjeta servicioTarjeta)
        {
            _servicioTarjeta = servicioTarjeta ?? throw new ArgumentNullException(nameof(servicioTarjeta));

            InicializarDatos();
        }

        #endregion CONSTRUCTOR

        #region INICIALIZACIÓN

        private void InicializarDatos()
        {
            RestablecerFormulario();
        }
        #endregion INICIALIZACIÓN

        #region MÉTODOS DE DATOS

        public async Task CargarTarjetasAsync()
        {
            try
            {
                IsLoading = true;
                HasError = false;

                var tarjetas = await _servicioTarjeta.ObtenerTodosAsync();

                ListaTarjetas.Clear();
                if (tarjetas?.Any() == true)
                {
                    foreach (var tarjeta in tarjetas.OrderByDescending(t => t.FechaCreacion))
                    {
                        ListaTarjetas.Add(tarjeta);
                    }
                }
            }
            catch (Exception ex)
            {
                await MostrarError("Error cargando tarjetas", ex.Message);
                HasError = true;
            }
            finally
            {
                IsLoading = false;
            }
        }

        // Propiedades calculadas para UI
        public bool TieneTarjetas => ListaTarjetas?.Count > 0;

        public bool NoTieneTarjetas => !TieneTarjetas;

        #endregion MÉTODOS DE DATOS

        #region COMMANDS

        [RelayCommand]
        public void ColorTarjetaSeleccionada(string colores)
        {
            var split = colores.Split('|');
            if (split.Length == 2)
            {
                LinearColor1 = split[0];
                LinearColor2 = split[1];
            }
        }

        [RelayCommand]
        public void IconoSeleccionado(string logoSeleccionado)
        {
            if (logoSeleccionado != null)
            {
                LogoTarjeta = logoSeleccionado;
            }
        }

        [RelayCommand]
        private void CambiarATarjetaDebito()
        {
            EsVisibleLimiteCredito = false;
            EsVisibleMonto = true;

            // Limpiar campo no usado
            LimiteCredito = string.Empty;

            AgregarTarjetaCommand.NotifyCanExecuteChanged();
        }

        [RelayCommand]
        private void CambiarATarjetaCredito()
        {
            EsVisibleLimiteCredito = true;
            EsVisibleMonto = false;

            // Limpiar campo no usado
            MontoInicial = string.Empty;

            OnPropertyChanged(nameof(TipoTarjetaSeleccionado));
            OnPropertyChanged(nameof(EsTarjetaCredito));
            OnPropertyChanged(nameof(EsTarjetaDebito));

            AgregarTarjetaCommand.NotifyCanExecuteChanged();
        }

        [RelayCommand(CanExecute = nameof(PuedeAgregarTarjeta))]
        private async Task AgregarTarjeta()
        {
            if (IsGuardando) return;

            try
            {
                IsGuardando = true;
                HasError = false;

                // Validación final
                if (!ValidarFormulario())
                {
                    return;
                }

                var nuevaTarjeta = new Tarjeta
                {
                    Nombre = NombreTarjeta.Trim(),
                    Ultimos4Digitos = UltimosCuatroDigitos.Trim(),
                    Tipo = EsVisibleMonto ? "Debito" : "Credito",
                    Banco = string.IsNullOrWhiteSpace(Banco) ? null : Banco.Trim(),
                    Vencimiento = string.IsNullOrWhiteSpace(Vencimiento) ? null : Vencimiento.Trim(),
                    LimiteCredito = EsVisibleLimiteCredito && double.TryParse(LimiteCredito, out var limite) ? limite : null,
                    MontoInicial = EsVisibleMonto && double.TryParse(MontoInicial, out var monto) ? monto : null,
                    Categoria = Categoria,
                    ColorHex1 = LinearColor1,
                    ColorHex2 = LinearColor2,
                    Logo = LogoTarjeta,
                    Descripcion = string.IsNullOrWhiteSpace(Descripcion) ? null : Descripcion.Trim()
                };

                var resultado = await _servicioTarjeta.InsertarAsync(nuevaTarjeta);

                if (resultado > 0)
                {
                    MostrarExito($"Tarjeta '{NombreTarjeta}' agregada correctamente");
                    RestablecerFormulario();
                }
                else
                {
                    await MostrarError("Error", "No se pudo agregar la tarjeta");
                }
            }
            catch (InvalidOperationException ex)
            {
                // Error de validación de negocio (ej: tarjeta duplicada)
                await MostrarError("Validación", ex.Message);
            }
            catch (Exception ex)
            {
                await MostrarError("Error inesperado", ex.Message);
            }
            finally
            {
                IsGuardando = false;
            }
        }
        /// <summary>
        /// Cierra el popup de información
        /// </summary>
        [RelayCommand]
        public void CerrarPopInformacion()
        {
            PopupInformacionAbierto = false; //CerrarPopInformacion
            _mensajeInformacion = string.Empty;
        }

        [RelayCommand]
        private async Task EliminarTarjeta(Tarjeta tarjeta)
        {
            if (tarjeta == null) return;

            var confirmar = await Shell.Current.DisplayAlert(
                "Confirmar eliminación",
                $"¿Estás seguro de eliminar la tarjeta '{tarjeta.Nombre}' terminada en {tarjeta.Ultimos4Digitos}?",
                "Eliminar", "Cancelar");

            if (!confirmar) return;

            try
            {
                IsLoading = true;
                var resultado = await _servicioTarjeta.EliminarAsync(tarjeta.Id);

                if (resultado > 0)
                {
                    ListaTarjetas.Remove(tarjeta);
                    OnPropertyChanged(nameof(TieneTarjetas));
                    OnPropertyChanged(nameof(NoTieneTarjetas));
                    MostrarExito("Tarjeta eliminada correctamente");
                }
                else
                {
                    await MostrarError("Error", "No se pudo eliminar la tarjeta");
                }
            }
            catch (Exception ex)
            {
                await MostrarError("Error eliminando tarjeta", ex.Message);
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private void RestablecerFormulario()
        {
            NombreTarjeta = string.Empty;
            UltimosCuatroDigitos = string.Empty;
            Banco = string.Empty;
            Vencimiento = string.Empty;
            LimiteCredito = string.Empty;
            MontoInicial = string.Empty;
            Descripcion = string.Empty;

            // Valores por defecto
            LinearColor1 = "#3E298F";
            LinearColor2 = "#836EDB";
            LogoTarjeta = "icono_visa.svg";
            Categoria = "Visa";
            EsVisibleLimiteCredito = true;
            EsVisibleMonto = false;

            HasError = false;
            MensajeError = string.Empty;
        }
        #endregion COMMANDS

        #region VALIDACIÓN

        private bool PuedeAgregarTarjeta()
        {
            return NombreTarjetaEsValido &&
                   UltimosCuatroDigitosEsValido &&
                   VencimientoEsValido &&
                   !IsGuardando;
        }

        private bool ValidarFormulario()
        {
            var errores = new List<string>();

            if (!NombreTarjetaEsValido)
                errores.Add("El nombre de la tarjeta debe tener al menos 2 caracteres");

            if (!UltimosCuatroDigitosEsValido)
                errores.Add("Los últimos 4 dígitos deben ser exactamente 4 números");

            if (!VencimientoEsValido)
                errores.Add("El formato de vencimiento debe ser MM/YY");

            if (errores.Any())
            {
                MensajeError = string.Join("\n", errores);
                HasError = true;
                return false;
            }

            return true;
        }

        private static bool EsFormatoVencimientoValido(string vencimiento)
        {
            if (string.IsNullOrEmpty(vencimiento) || vencimiento.Length != 5 || !vencimiento.Contains('/'))
                return false;

            var partes = vencimiento.Split('/');
            if (partes.Length != 2)
                return false;

            return int.TryParse(partes[0], out int mes) && mes >= 1 && mes <= 12 &&
                   int.TryParse(partes[1], out int año) && año >= 0 && año <= 99;
        }

        #endregion VALIDACIÓN

        #region HELPERS

        private async Task MostrarError(string titulo, string mensaje)
        {
            await Shell.Current.DisplayAlert(titulo, mensaje, "OK");
        }

        private void MostrarExito(string mensaje)
        {
            MensajeInformacion = mensaje;
            PopupInformacionAbierto = true;
        }

        #endregion HELPERS
    }
}