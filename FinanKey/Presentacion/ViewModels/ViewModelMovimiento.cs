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

        #endregion PROPIEDADES DEL FORMULARIO

        #region COLECCIONES

        [ObservableProperty]
        private ObservableCollection<CategoriaMovimiento> _listaTipoCategoriasGastos = [];

        [ObservableProperty]
        private ObservableCollection<CategoriaMovimiento> _listaTipoCategoriasIngresos = [];

        [ObservableProperty]
        private ObservableCollection<CategoriaMovimiento> _listaCategoriasActual = [];

        [ObservableProperty]
        private ObservableCollection<Tarjeta> _listaTarjetas = [];

        [ObservableProperty]
        private ObservableCollection<Tarjeta> _listaTarjetasDebito = [];

        [ObservableProperty]
        private ObservableCollection<Tarjeta> _listaTarjetasCredito = [];

        #endregion COLECCIONES

        #region CONSTRUCTOR

        public ViewModelMovimiento(ServicioMovimiento servicioMovimiento)
        {
            _servicioMovimiento = servicioMovimiento ?? throw new ArgumentNullException(nameof(servicioMovimiento));
        }

        #endregion CONSTRUCTOR

        #region INICIALIZACIÓN
        /// <summary>
        /// Metodo para inicializar los datos
        /// inicializa las listas de tarjetas y categorias
        /// </summary>
        /// <returns></returns>
        public async Task InicializarDatosAsync()
        {
            try
            {
                // Mostrar el indicador de carga
                IsBusy = true;
                // Limpiar mensajes de error
                HasError = false;
                // Tarea se encarga de cargar todos los metodos asincronos de carga de tajetas y categorias
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
        /// <summary>
        /// Metodo para cargar las tarjetas
        /// lo que buscamos es cargar la lista de tarjetas 
        /// asignamos la consulta a la base de datos
        /// y luego repartimos esa consulta segun sea de credito o debito
        /// </summary>
        /// <returns></returns>
        private async Task CargarTarjetasAsync()
        {
            try
            {
                //Consulta a la base de datos, trae todas las tarjetas
                var tarjetas = await _servicioMovimiento.obtenerTarjetas();
                //Si no hay nada en la lista de tarjetas, salimos del metodo, para evitar errores con valores nulos
                if(tarjetas == null) return;
                //Limpiamos la lista actual de tarjetas
                ListaTarjetas.Clear();
                ListaTarjetas = new ObservableCollection<Tarjeta>(tarjetas);
                //Inicializamos la lista de tarjetas de debito
                ListaTarjetasDebito.Clear();
                //De la lista de tarjetas, filtramos las tarjetas de debito y las asignamos a la lista de tarjetas de debito
                ListaTarjetasDebito = new ObservableCollection<Tarjeta>(ListaTarjetas.Where(td => td?.Tipo == "Debito"));
                //inicializamos la lista de tarjetas de credito
                ListaTarjetasCredito.Clear();
                //De la lista de tarjetas, filtramos las tarjetas de credito y las asignamos a la lista de tarjetas de credito
                ListaTarjetasCredito = new ObservableCollection<Tarjeta>(ListaTarjetas.Where(td => td?.Tipo == "Credito"));
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
            try
            {
                EsGastoSeleccionado = true;
                EsIngresoSeleccionado = false;
                ActualizarListaCategorias();
                LimpiarCategoria();
                ListaTarjetasDebito.Clear();
                ListaTarjetasDebito = new ObservableCollection<Tarjeta>(ListaTarjetas.Where(t => t?.Tipo == "Debito"));
            }
            catch (Exception ex)
            {
                MostrarError("Error cambiando pestaña", ex.Message);
            }
        }

        [RelayCommand]
        private void SeleccionarIngreso()
        {
            try
            {
                EsGastoSeleccionado = false;
                EsIngresoSeleccionado = true;
                ActualizarListaCategorias();
                LimpiarCategoria();
                ListaTarjetasDebito.Clear();
                ListaTarjetasDebito = new ObservableCollection<Tarjeta>(ListaTarjetas.Where(t => t?.Tipo == "Credito"));
            }
            catch (Exception ex)
            {
                MostrarError("Error cambiando pestaña", ex.Message);
            }
        }

        private void LimpiarCategoria()
        {
            CategoriaSeleccionada = null;
        }

        private void ActualizarListaCategorias()
        {
            try
            {
                ListaCategoriasActual.Clear();

                var categorias = EsGastoSeleccionado ? ListaTipoCategoriasGastos : ListaTipoCategoriasIngresos;

                if (categorias == null) return;

                foreach (var categoria in categorias)
                {
                    ListaCategoriasActual.Add(categoria);
                }
            }
            catch (Exception ex)
            {
                MostrarError("Error actualizando categorías", ex.Message);
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

                //actualizar credito usado si el movimiento fue para tarjeta de credito
                if(resultado > 0 && movimiento.Tarjeta?.Tipo == "Credito")
                {
                    //calcular credito usado
                    CalcularCreditoUsado();
                    //Actualizar credito usado
                    var resultadoCreditoUsado = await _servicioMovimiento.ActualizarAsync(movimiento.Tarjeta);
                }

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
            try
            {
                // limpiar
                Monto = 0;
                Fecha = DateTime.Now;
                Descripcion = string.Empty;
                CategoriaSeleccionada = null;
                TarjetaSeleccionada = null;
                Comercio = string.Empty;
                EstaPagado = false;

                if (movimiento == "Ingreso")
                {
                    ListaCategoriasActual = ListaTipoCategoriasIngresos ?? new ObservableCollection<CategoriaMovimiento>();
                    EsGastoSeleccionado = false;

                    ListaTarjetasDebito.Clear();
                    foreach (Tarjeta tarjeta in ListaTarjetas ?? new ObservableCollection<Tarjeta>())
                    {
                        if (tarjeta?.Tipo == "Debito")
                        {
                            ListaTarjetasDebito.Add(tarjeta);
                            EsGastoSeleccionado = true;
                        }
                    }
                }
                else
                {
                    ListaCategoriasActual = ListaTipoCategoriasGastos ?? new ObservableCollection<CategoriaMovimiento>();
                }
            }
            catch (Exception ex)
            {
                MostrarError("Error inicializando formulario", ex.Message);
            }
        }


        #endregion INICIALIZACIÓN FORMULARIO CUANDO SE CAMBIA DE PANTALLA

        #region HELPERS
        /// <summary>
        /// Creacion de una instacia de movimiento
        /// con los datos del formulario
        /// </summary>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        private Movimiento CrearMovimiento()
        {
            try
            {
                if (CategoriaSeleccionada == null || TarjetaSeleccionada == null)
                    throw new InvalidOperationException("Debe seleccionar una categoría y una tarjeta");

                return new Movimiento
                {
                    TipoMovimiento = TipoMovimientoActual,
                    Monto = Monto,
                    Descripcion = Descripcion?.Trim() ?? string.Empty,
                    FechaMovimiento = Fecha,
                    CategoriaId = CategoriaSeleccionada.Id,
                    TarjetaId = TarjetaSeleccionada.Id,
                    Comercio = Comercio?.Trim() ?? string.Empty,
                    EsPagado = EstaPagado,
                    MedioPago = "Tarjeta",
                    BorderFondoEstado = ObtenerColorFondo(),
                    ColorFuenteEstado = ObtenerColorTexto()
                };
            }
            catch (Exception ex)
            {
                MostrarError("Error creando movimiento", ex.Message);
                throw; // relanzas para que GuardarMovimiento lo capture también
            }
        }

        /// <summary>
        /// Calcula el crédito usado de la tarjeta seleccionada
        /// Antes de guardar el movimiento
        /// </summary>
        private void CalcularCreditoUsado()
        {
            try
            {
                if (TarjetaSeleccionada == null || TarjetaSeleccionada.Tipo != "Credito")
                    return;
                // Lógica para actualizar el crédito usado
                // Esto es un ejemplo, ajusta según tu lógica de negocio
                double? nuevoCreditoUsado = TarjetaSeleccionada.CreditoUsado + Monto;
                if (nuevoCreditoUsado > TarjetaSeleccionada.LimiteCredito)
                {
                    MostrarError("Error", "El monto excede el límite de crédito disponible.");
                    return;
                }
                //Se actualiza el crédito usado de la tarjeta seleccionada
                TarjetaSeleccionada.CreditoUsado = nuevoCreditoUsado;
            }
            catch (Exception ex)
            {
                MostrarError("Error actualizando crédito usado", ex.Message);
            }
        }

        private string ObtenerColorFondo()
        {
            try
            {
                if (TarjetaSeleccionada?.Tipo == "Credito" && !EstaPagado)
                    return "#FEF3C7";

                return EstaPagado ? "#DCFCE7" : "#FEF2F2";
            }
            catch
            {
                return "#FFFFFF"; // fallback
            }
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