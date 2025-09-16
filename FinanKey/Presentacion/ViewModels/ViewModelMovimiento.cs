using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FinanKey.Aplicacion.UseCases;
using FinanKey.Dominio.Models;
using System.Collections.ObjectModel;

namespace FinanKey.Presentacion.ViewModels
{
    public partial class ViewModelMovimiento : ObservableObject
    {
        #region DEPENDENCIAS

        private readonly ServicioMovimiento _servicioMovimiento;

        #endregion DEPENDENCIAS

        #region ESTADOS DE UI

        [ObservableProperty]
        private bool _isBusy;

        [ObservableProperty]
        private bool _isBottomSheetOpen;

        [ObservableProperty]
        private bool _isGuardando;

        [ObservableProperty]
        private bool _hasError;

        [ObservableProperty]
        private string _mensajeError = string.Empty;

        [ObservableProperty]
        private string _tituloError = string.Empty;

        // Control de pestañas/formularios
        [ObservableProperty]
        private bool _esGastoSeleccionado = true;

        [ObservableProperty]
        private bool _esIngresoSeleccionado = false;

        [ObservableProperty]
        private bool _popupInformacionAbierto;

        [ObservableProperty]
        private bool _popupErrorAbierto;

        [ObservableProperty]
        private string? _mensajeInformacion;

        [ObservableProperty]
        private string? _estado;

        #endregion ESTADOS DE UI

        #region PROPIEDADES DEL FORMULARIO

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(GuardarMovimientoCommand))]
        private double _monto = 0;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(GuardarMovimientoCommand))]
        private string _descripcion = string.Empty;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(GuardarMovimientoCommand))]
        private CategoriaMovimiento? _categoriaSeleccionada;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(GuardarMovimientoCommand))]
        private Tarjeta? _tarjetaSeleccionada;

        [ObservableProperty]
        private DateTime _fecha = DateTime.Now;

        [ObservableProperty]
        private string _comercio = string.Empty;

        [ObservableProperty]
        private bool _estaPagado = false;

        // Propiedades calculadas
        public string TipoMovimientoActual => EsGastoSeleccionado ? "Gasto" : "Ingreso";

        public string TextoBotonGuardar => $"Guardar {TipoMovimientoActual}";

        #endregion PROPIEDADES DEL FORMULARIO

        #region COLECCIONES

        [ObservableProperty]
        private ObservableCollection<CategoriaMovimiento> _listaTipoCategoriasGastos = new();

        [ObservableProperty]
        private ObservableCollection<CategoriaMovimiento> _listaTipoCategoriasIngresos = new();

        [ObservableProperty]
        private ObservableCollection<CategoriaMovimiento> _listaCategoriasActual = new();

        [ObservableProperty]
        private ObservableCollection<Tarjeta> _listaTarjetas = new();

        [ObservableProperty]
        private ObservableCollection<Tarjeta> _listaTarjetasDebito = new();

        // Propiedades calculadas para UI
        public bool TieneCategorias => ListaCategoriasActual?.Count > 0;

        public bool TieneTarjetas => ListaTarjetas?.Count > 0;

        #endregion COLECCIONES

        #region CONSTRUCTOR

        public ViewModelMovimiento(ServicioMovimiento servicioMovimiento)
        {
            _servicioMovimiento = servicioMovimiento ?? throw new ArgumentNullException(nameof(servicioMovimiento));
        }

        #endregion CONSTRUCTOR

        #region INICIALIZACIÓN

        public async Task InicializarDatosAsync()
        {
            try
            {
                IsBusy = true;
                HasError = false;

                await Task.WhenAll(
                    CargarTarjetasAsync(),
                    CargarCategoriasGastosAsync(),
                    CargarCategoriasIngresosAsync()
                );

                // Establecer lista inicial (gastos por defecto)
                ActualizarListaCategorias();
            }
            catch (Exception ex)
            {
                HasError = true;
                MensajeError = $"Error inicializando datos: {ex.Message}";
                MostrarError("Error de inicialización", ex.Message);
            }
            finally
            {
                IsBusy = false;
            }
        }

        #endregion INICIALIZACIÓN

        #region CARGA DE DATOS ASYNCRONO

        private async Task CargarTarjetasAsync()
        {
            try
            {
                var tarjetas = await _servicioMovimiento.obtenerTarjetas();

                ListaTarjetas.Clear();
                foreach (var tarjeta in tarjetas ?? new List<Tarjeta>())
                {
                    ListaTarjetas.Add(tarjeta);
                }
            }
            catch (Exception ex)
            {
                MensajeErrorExepcion($"Error cargando tarjetas: {ex.Message}", ex);
            }
        }

        private async Task CargarCategoriasGastosAsync()
        {
            try
            {
                var categoriasGastos = await _servicioMovimiento.ObtenerCategoriasTipoGastosAsync();

                ListaTipoCategoriasGastos.Clear();
                foreach (var categoria in categoriasGastos)
                {
                    ListaTipoCategoriasGastos.Add(categoria ?? new CategoriaMovimiento());
                }
            }
            catch (Exception ex)
            {
                MensajeErrorExepcion($"Error cargando categorías de gastos: {ex.Message}", ex);
            }
        }

        private async Task CargarCategoriasIngresosAsync()
        {
            try
            {
                var categoriasIngresos = await _servicioMovimiento.ObtenerCategoriasTipoIngresosAsync();

                ListaTipoCategoriasIngresos.Clear();
                foreach (var categoria in categoriasIngresos)
                {
                    ListaTipoCategoriasIngresos.Add(categoria ?? new CategoriaMovimiento());
                }
            }
            catch (Exception ex)
            {
                MensajeErrorExepcion($"Error cargando categorías de ingresos: {ex.Message}", ex);
            }
        }

        #endregion CARGA DE DATOS ASYNCRONO

        #region COMMANDS - NAVEGACIÓN

        [RelayCommand]
        private void SeleccionarGasto()
        {
            EsGastoSeleccionado = true;
            EsIngresoSeleccionado = false;
            ActualizarListaCategorias();
            LimpiarCategoria();
        }

        [RelayCommand]
        private void SeleccionarIngreso()
        {
            EsGastoSeleccionado = false;
            EsIngresoSeleccionado = true;
            ActualizarListaCategorias();
            LimpiarCategoria();
        }

        private void LimpiarCategoria()
        {
            CategoriaSeleccionada = null;
        }

        private void ActualizarListaCategorias()
        {
            ListaCategoriasActual.Clear();

            var categorias = EsGastoSeleccionado ? ListaTipoCategoriasGastos : ListaTipoCategoriasIngresos;

            foreach (var categoria in categorias ?? new ObservableCollection<CategoriaMovimiento>())
            {
                ListaCategoriasActual.Add(categoria);
            }
        }

        #endregion COMMANDS - NAVEGACIÓN

        #region COMMANDS - CATEGORIAS

        [RelayCommand]
        private void MostrarBottomSheetCategorias()
        {
            if (Estado == "Collapsed" || Estado == null)
            {
                if (IsBottomSheetOpen)
                {
                    IsBottomSheetOpen = false;
                    return;
                }
                if (!IsBottomSheetOpen) IsBottomSheetOpen = true;
            }
        }

        [RelayCommand]
        private void SeleccionarCategoria(CategoriaMovimiento categoria)
        {
            if (categoria == null) return;

            CategoriaSeleccionada = categoria;
            IsBottomSheetOpen = false;
        }

        [RelayCommand]
        private void CerrarBottomSheet()
        {
            IsBottomSheetOpen = false;
        }

        #endregion COMMANDS - CATEGORIAS

        #region COMMANDS - GUARDAR

        [RelayCommand(CanExecute = nameof(PuedeGuardarMovimiento))]
        private async Task GuardarMovimiento()
        {
            if (IsGuardando) return;

            try
            {
                IsBusy = true;
                IsGuardando = true;
                HasError = false;

                if (!ValidarFormulario())
                {
                    return;
                }

                var movimiento = CrearMovimiento();
                var resultado = await _servicioMovimiento.guardarMovimiento(movimiento);

                if (resultado > 0)
                {
                    MostrarExito($"{TipoMovimientoActual} guardado correctamente");
                    LimpiarFormulario();
                }
                else
                {
                    MostrarError("Error", $"No se pudo guardar el {TipoMovimientoActual.ToLower()}");
                }
            }
            catch (Exception ex)
            {
                MostrarError($"Error guardando {TipoMovimientoActual.ToLower()}", ex.Message);
                HasError = true;
                MensajeError = ex.Message;
            }
            finally
            {
                IsGuardando = false;
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task RefrescarDatos()
        {
            await InicializarDatosAsync();
        }

        #endregion COMMANDS - GUARDAR

        #region COMANDO CERRAR VENTANA EMERGENTE

        [RelayCommand]
        private void CerrarPopInformacion()
        {
            PopupInformacionAbierto = false;
            MensajeInformacion = string.Empty;
        }

        [RelayCommand]
        private void CerrarPopError()
        {
            PopupErrorAbierto = false;
            MensajeError = string.Empty;
        }

        #endregion COMANDO CERRAR VENTANA EMERGENTE

        #region VALIDACIÓN

        private bool PuedeGuardarMovimiento()
        {
            return !IsGuardando &&
                   Monto > 0 &&
                   !string.IsNullOrWhiteSpace(Descripcion) &&
                   CategoriaSeleccionada != null &&
                   TarjetaSeleccionada != null;
        }

        private bool ValidarFormulario()
        {
            var errores = new List<string>();

            if (Monto <= 0)
                errores.Add("El monto debe ser mayor a 0");

            if (string.IsNullOrWhiteSpace(Descripcion))
                errores.Add("La descripción es requerida");
            else if (Descripcion.Length > 100)
                errores.Add("La descripción no puede exceder 100 caracteres");

            if (CategoriaSeleccionada == null)
                errores.Add("Debe seleccionar una categoría");

            if (TarjetaSeleccionada == null)
                errores.Add("Debe seleccionar una tarjeta");

            if (Fecha > DateTime.Now)
                errores.Add("La fecha no puede ser futura");

            if (errores.Any())
            {
                HasError = true;
                MensajeError = string.Join("\n", errores);
                return false;
            }

            return true;
        }

        #endregion VALIDACIÓN

        #region INICIALIZACIÓN FORMULARIO CUANDO SE CAMBIA DE PANTALLA

        [RelayCommand]
        private void InicializarDatosFormulario(string movimiento)
        {
            //LIMPIAR CAMPOS
            Monto = 0;
            Fecha = DateTime.Now;
            Descripcion = string.Empty;
            CategoriaSeleccionada = null;
            TarjetaSeleccionada = null;
            Comercio = string.Empty;
            EstaPagado = false;

            if (movimiento == "Ingreso")
            {
                ListaCategoriasActual = ListaTipoCategoriasIngresos;
                EsGastoSeleccionado = false;
                ListaTarjetasDebito.Clear();
                foreach (Tarjeta tarjeta in ListaTarjetas)
                {
                    if (tarjeta.Tipo == "Debito")
                    {
                        ListaTarjetasDebito.Add(tarjeta);
                        EsGastoSeleccionado = true;
                    }
                }
            }
            else
            {
                ListaCategoriasActual = ListaTipoCategoriasGastos;
            }
        }

        #endregion INICIALIZACIÓN FORMULARIO CUANDO SE CAMBIA DE PANTALLA

        #region HELPERS

        private Movimiento CrearMovimiento()
        {
            return new Movimiento
            {
                TipoMovimiento = TipoMovimientoActual,
                Monto = Monto,
                Descripcion = Descripcion.Trim(),
                FechaMovimiento = Fecha,
                CategoriaId = CategoriaSeleccionada!.Id,
                TarjetaId = TarjetaSeleccionada!.Id,
                Comercio = Comercio.Trim(),
                EsPagado = EstaPagado,
                MedioPago = "Tarjeta",
                // Colores basados en estado y tipo de tarjeta
                BorderFondoEstado = ObtenerColorFondo(),
                ColorFuenteEstado = ObtenerColorTexto()
            };
        }

        private string ObtenerColorFondo()
        {
            if (TarjetaSeleccionada?.Tipo == "Credito" && !EstaPagado)
                return "#FEF3C7"; // Amarillo claro para pendientes

            return EstaPagado ? "#DCFCE7" : "#FEF2F2"; // Verde claro para pagados, rojo claro para pendientes
        }

        private string ObtenerColorTexto()
        {
            if (TarjetaSeleccionada?.Tipo == "Credito" && !EstaPagado)
                return "#A16207"; // Amarillo oscuro para pendientes

            return EstaPagado ? "#166534" : "#DC2626"; // Verde oscuro para pagados, rojo oscuro para pendientes
        }

        private void LimpiarFormulario()
        {
            Monto = 0;
            Descripcion = string.Empty;
            Comercio = string.Empty;
            CategoriaSeleccionada = null;
            TarjetaSeleccionada = null;
            EstaPagado = false;
            Fecha = DateTime.Now;
            HasError = false;
            MensajeError = string.Empty;
        }

        private void MostrarError(string titulo, string mensaje)
        {
            PopupErrorAbierto = true;
            TituloError = titulo;
            MensajeError = mensaje;
        }

        private void MensajeErrorExepcion(string mensaje, Exception ex)
        {
            PopupErrorAbierto = true;
            TituloError = "Error";
            MensajeError = $"{mensaje}: {ex.Message}";
        }

        private void MostrarExito(string mensaje)
        {
            PopupInformacionAbierto = true;
            MensajeInformacion = mensaje;
        }

        #endregion HELPERS
    }
}