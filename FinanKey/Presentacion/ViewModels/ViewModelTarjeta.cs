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
        #endregion

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

        // Propiedades calculadas para el tipo de tarjeta
        public string TipoTarjetaSeleccionado => EsVisibleMonto ? "Débito" : "Crédito";
        public bool EsTarjetaCredito => !EsVisibleMonto;
        public bool EsTarjetaDebito => EsVisibleMonto;
        #endregion

        #region PROPIEDADES DE DISEÑO
        [ObservableProperty]
        private string _logoTarjeta = "icono_visa.svg";

        [ObservableProperty]
        private string _categoria = "Visa";

        [ObservableProperty]
        private string _linearColor1 = "#3E298F";

        [ObservableProperty]
        private string _linearColor2 = "#836EDB";
        #endregion

        #region COLECCIONES
        [ObservableProperty]
        private ObservableCollection<Tarjeta> _listaTarjetas = new();

        public ObservableCollection<OpcionTarjeta> ListaLogoTarjeta { get; private set; } = new();

        public ObservableCollection<GradienteColor> ListaGradientes { get; private set; } = new();
        #endregion

        #region PROPIEDADES DE VALIDACIÓN
        public bool NombreTarjetaEsValido => !string.IsNullOrWhiteSpace(NombreTarjeta) && NombreTarjeta.Length >= 2;
        public bool UltimosCuatroDigitosEsValido => UltimosCuatroDigitos?.Length == 4;
        public bool VencimientoEsValido => string.IsNullOrEmpty(Vencimiento) || EsFormatoVencimientoValido(Vencimiento);
        public bool MontoEsValido => EsVisibleMonto ? EsMontoValido(MontoInicial) : EsMontoValido(LimiteCredito);
        #endregion

        #region CONSTRUCTOR
        public ViewModelTarjeta(ServicioTarjeta servicioTarjeta)
        {
            _servicioTarjeta = servicioTarjeta ?? throw new ArgumentNullException(nameof(servicioTarjeta));

            InicializarDatos();

            //// Cargar datos iniciales en background
            //_ = Task.Run(async () => await CargarDatosInicialesAsync());
        }
        #endregion

        #region INICIALIZACIÓN
        private void InicializarDatos()
        {
            InicializarLogos();
            InicializarGradientes();
            RestablecerFormulario();
        }

        private void InicializarLogos()
        {
            ListaLogoTarjeta.Clear();
            var logos = new[]
            {
                new OpcionTarjeta { Icono = "icono_visa.svg", Nombre = "Visa", Categoria = "Visa" },
                new OpcionTarjeta { Icono = "icono_master_card.svg", Nombre = "Mastercard", Categoria = "Mastercard" },
                new OpcionTarjeta { Icono = "icono_american_express.svg", Nombre = "American Express", Categoria = "American_Express" }
            };

            foreach (var logo in logos)
            {
                ListaLogoTarjeta.Add(logo);
            }
        }

        private void InicializarGradientes()
        {
            ListaGradientes.Clear();
            var gradientes = new[]
            {
                new GradienteColor { Color1 = "#3E298F", Color2 = "#836EDB", Nombre = "Púrpura Clásico" },
                new GradienteColor { Color1 = "#FF6B6B", Color2 = "#FF8E8E", Nombre = "Coral Suave" },
                new GradienteColor { Color1 = "#4ECDC4", Color2 = "#44A08D", Nombre = "Verde Aguamarina" },
                new GradienteColor { Color1 = "#45B7D1", Color2 = "#96CEB4", Nombre = "Azul Océano" },
                new GradienteColor { Color1 = "#FFA726", Color2 = "#FFCC02", Nombre = "Naranja Dorado" },
                new GradienteColor { Color1 = "#667eea", Color2 = "#764ba2", Nombre = "Azul Violeta" },
                new GradienteColor { Color1 = "#f093fb", Color2 = "#f5576c", Nombre = "Rosa Vibrante" },
                new GradienteColor { Color1 = "#4facfe", Color2 = "#00f2fe", Nombre = "Azul Cyan" }
            };

            foreach (var gradiente in gradientes)
            {
                ListaGradientes.Add(gradiente);
            }
        }

        private async Task CargarDatosInicialesAsync()
        {
            try
            {
                IsLoading = true;
                await CargarTarjetasAsync();
            }
            catch (Exception ex)
            {
                await MostrarError("Error cargando datos iniciales", ex.Message);
            }
            finally
            {
                IsLoading = false;
            }
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
        #endregion

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
                    await MostrarExito($"Tarjeta '{NombreTarjeta}' agregada correctamente");
                    RestablecerFormulario();
                    await CargarTarjetasAsync();
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
                    await MostrarExito("Tarjeta eliminada correctamente");
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

        [RelayCommand]
        private async Task RefrescarTarjetas()
        {
            await CargarTarjetasAsync();
        }
        #endregion

        #region VALIDACIÓN
        private bool PuedeAgregarTarjeta()
        {
            return NombreTarjetaEsValido &&
                   UltimosCuatroDigitosEsValido &&
                   VencimientoEsValido &&
                   MontoEsValido &&
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

            if (!MontoEsValido)
            {
                var campo = EsVisibleMonto ? "monto inicial" : "límite de crédito";
                errores.Add($"El {campo} debe ser un número válido mayor a 0");
            }

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

        private static bool EsMontoValido(string monto)
        {
            return double.TryParse(monto, out double valor) && valor > 0;
        }
        #endregion

        #region HELPERS
        private async Task MostrarError(string titulo, string mensaje)
        {
            await Shell.Current.DisplayAlert(titulo, mensaje, "OK");
        }

        private async Task MostrarExito(string mensaje)
        {
            await Shell.Current.DisplayAlert("Éxito", mensaje, "OK");
        }
        #endregion
    }

    #region MODELOS DE APOYO
    public class OpcionTarjeta
    {
        public string Icono { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string Categoria { get; set; } = string.Empty;
    }

    public class GradienteColor
    {
        public string Color1 { get; set; } = string.Empty;
        public string Color2 { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string ColoresString => $"{Color1}|{Color2}";
    }
    #endregion
}