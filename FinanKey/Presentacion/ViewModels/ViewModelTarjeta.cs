using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FinanKey.Aplicacion.UseCases;
using FinanKey.Aplicacion.Validation;
using FinanKey.Dominio.Models;
using System.Collections.ObjectModel;

namespace FinanKey.Presentacion.ViewModels
{
    public partial class ViewModelTarjeta : ObservableObject
    {
        #region INYECCION DE DEPENDENCIAS

        private readonly ServicioTarjeta _servicioTarjeta;

        #endregion DEPENDENCIAS

        #region DECLARACION DE PROPIEDADES DEVALIDACIONES
        public ValidatableObject<string> NombreTarjeta { get; private set; }
        public ValidatableObject<string> UltimosCuatroDigitos { get; private set; }
        public ValidatableObject<string> Banco { get; private set; }
        public ValidatableObject<string> FechaVencimiento { get; private set; }
        #endregion

        #region METODOS DEL PROYECTO PARA VALIDACIONES
        private void InitializeValidatableObjects()
        {
            NombreTarjeta = new ValidatableObject<string>();
            UltimosCuatroDigitos = new ValidatableObject<string>();
            Banco = new ValidatableObject<string>();
            FechaVencimiento = new ValidatableObject<string>();
        }
        private void AgregaValidaciones()
        {
            NombreTarjeta.Validations.Add(new IsNotNullOrEmptyRule<string>
            {
                ValidationMessage = "El nombre de la tarjeta es requerido"
            });
            NombreTarjeta.Validations.Add(new MinLengthRule<string>
            {
                MinLength = 3,
                ValidationMessage = "Debe tener al menos 3 caracteres."
            });
            UltimosCuatroDigitos.Validations.Add(new IsNotNullOrEmptyRule<string>
            {
                ValidationMessage = "Los ultimos cuatro digitos son requeridos"
            });
            UltimosCuatroDigitos.Validations.Add(new MinLengthRule<string>
            {
                MinLength = 4,
                ValidationMessage = "Los cuatro ultimos digitos son requeridos"
            });
            Banco.Validations.Add(new IsNotNullOrEmptyRule<string>
            {
                ValidationMessage = "Debes ingresar el banco o entidad financiera"
            });
            Banco.Validations.Add(new MinLengthRule<string>
            {
                MinLength = 3,
                ValidationMessage = "Debe tener al menos 3 caracteres."
            });
            FechaVencimiento.Validations.Add(new IsNotNullOrEmptyRule<string>
            {
                ValidationMessage = "La fecha de vencimiento es requerida"
            });
            FechaVencimiento.Validations.Add(new MinLengthRule<string>
            {
                MinLength = 5,
                ValidationMessage = "Debe tener al menos 4 caracteres"
            });
            FechaVencimiento.Validations.Add(new ReglaFechaTarjeta<string>
            {
                ValidationMessage = "El formato de vencimiento debe ser MM/YY"
            });
        }
        private bool ValidarTodos()
        {
            var validationResults = new[]
            {
                NombreTarjeta.Validate(),
                UltimosCuatroDigitos.Validate(),
                Banco.Validate(),
                FechaVencimiento.Validate()
            };

            return validationResults.All(result => result);
        }
        #endregion

        #region COMANDOS VALIDACION DE ENTRADAS
        [RelayCommand]
        private void ValidarNombreTarjeta() => NombreTarjeta.Validate();

        [RelayCommand]
        private void ValidarUltimosCuatroDigitos() => UltimosCuatroDigitos.Validate();

        [RelayCommand]
        private void ValidarBanco() => Banco.Validate();

        [RelayCommand]
        private void ValidarFechaVencimiento() => FechaVencimiento.Validate();
        #endregion

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

        #region CONSTRUCTOR
        public ViewModelTarjeta(ServicioTarjeta servicioTarjeta)
        {
            _servicioTarjeta = servicioTarjeta ?? throw new ArgumentNullException(nameof(servicioTarjeta));

            InitializeValidatableObjects();
            AgregaValidaciones();

            InicializarDatos();
        }
      
        #endregion CONSTRUCTOR

        #region INICIALIZACIÓN
        private void InicializarDatos()
        {
            RestablecerFormulario();
        }
        #endregion

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

        [RelayCommand]
        private async Task AgregarTarjeta()
        {
            if (IsGuardando) return;

            try
            {
                IsGuardando = true;
                HasError = false;

                if (ValidarTodos())
                {
                    var nuevaTarjeta = new Tarjeta
                    {
                        Nombre = NombreTarjeta.Value,
                        Ultimos4Digitos = UltimosCuatroDigitos.Value,
                        Tipo = EsVisibleMonto ? "Debito" : "Credito",
                        Banco = Banco.Value,
                        Vencimiento = FechaVencimiento.Value,
                        LimiteCredito = EsVisibleLimiteCredito && double.TryParse(LimiteCredito, out var limite) ? limite : null,
                        CreditoUsado = 0, // Nuevo, no tiene crédito usado
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
        private void RestablecerFormulario()
        {

            NombreTarjeta.Value = string.Empty;
            UltimosCuatroDigitos.Value = string.Empty;
            Banco.Value = string.Empty;
            FechaVencimiento.Value = string.Empty;
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

        private bool ValidarFormulario()
        {
            var errores = new List<string>();

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