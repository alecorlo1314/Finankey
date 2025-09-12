using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FinanKey.Dominio.Models;
using FinanKey.Aplicacion.UseCases;
using System.Collections.ObjectModel;

namespace FinanKey.Presentacion.ViewModels
{
    public partial class ViewModelMovimiento : ObservableObject
    {
        #region Inyeccion de Dependencias

        private readonly ServicioMovimiento _servicioMovimiento;

        #endregion Inyeccion de Dependencias

        #region Propiedades

        [ObservableProperty]
        public double _monto = 0;

        [ObservableProperty]
        public CategoriaMovimiento? _categoriaSeleccionada;

        [ObservableProperty]
        public DateTime _fecha = DateTime.Now;

        [ObservableProperty]
        public string? _comercio;

        [ObservableProperty]
        public Tarjeta? _tarjetaSeleccionada;

        [ObservableProperty]
        public bool? _estaPagado = false;

        [ObservableProperty]
        public string? _descripcion;

        #endregion Propiedades

        private string fondoColorBorder;//Revisar Propiedad por que no se usa
        private string fondoColorTexto; //Revisar Propiedad por que no se usa

        #region Listas o colecciones

        [ObservableProperty]
        public ObservableCollection<CategoriaMovimiento>? _listaTipoCategoriasGastos;

        [ObservableProperty]
        public ObservableCollection<CategoriaMovimiento>? _listaTipoCategoriasIngresos;

        [ObservableProperty]
        public ObservableCollection<CategoriaMovimiento>? _listaCategoriasActual;

        [ObservableProperty]
        public ObservableCollection<Tarjeta>? _listaTarjetas = new();

        #endregion Listas o colecciones

        #region Propiedades de control de UI

        [ObservableProperty]
        public bool _isBusy;

        [ObservableProperty]
        public bool _isBottomSheetOpen;

        #endregion Propiedades de control de UI

        #region Constructor

        public ViewModelMovimiento(ServicioMovimiento servicioMovimiento)
        {
            inicializarDatos();
            _servicioMovimiento = servicioMovimiento;
        }

        #endregion Constructor

        #region Metodo para inicializar todos los datos
        public Task inicializarDatos()
        {
            IsBusy = true;
            _ = CargarTarjetasAsync();
            _ = CargarCategoriasGastosAsync();
            _ = CargarCategoriasIngresosAsync();
            IsBusy = false;
            return Task.CompletedTask;
        }
        #endregion Metodo para inicializar todos los datos

        #region Metodos Cargan lista de tarjetas
        private async Task CargarTarjetasAsync()
        {
            try
            {
                IsBusy = true;
                var tarjetas = await _servicioMovimiento.obtenerTarjetas();

                if (!TarjetasIguales(tarjetas))
                {
                    ListaTarjetas?.Clear();
                    ListaTarjetas = new ObservableCollection<Tarjeta>(tarjetas);
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Error al cargar tarjetas: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private bool TarjetasIguales(List<Tarjeta> tarjetasNuevas)
        {
            if (ListaTarjetas?.Count != tarjetasNuevas.Count) return false;

            return ListaTarjetas.Zip(tarjetasNuevas, (actual, nuevo) =>
                actual.Id == nuevo.Id &&
                actual.Nombre == nuevo.Nombre &&
                actual.Ultimos4Digitos == nuevo.Ultimos4Digitos &&
                actual.Tipo == nuevo.Tipo &&
                actual.Banco == nuevo.Banco &&
                actual.Vencimiento == nuevo.Vencimiento &&
                actual.LimiteCredito == nuevo.LimiteCredito &&
                actual.MontoInicial == nuevo.MontoInicial &&
                actual.Categoria == nuevo.Categoria &&
                actual.Logo == nuevo.Logo &&
                actual.Descripcion == nuevo.Descripcion &&
                actual.ColorHex1 == nuevo.ColorHex1 &&
                actual.ColorHex2 == nuevo.ColorHex2 &&
                actual.FechaCreacion == nuevo.FechaCreacion)
                .All(iguales => iguales);
        }
        #endregion Metodos Cargan lista de tarjetas

        #region Metodo para cargar las categorias de gastos

        private async Task CargarCategoriasGastosAsync()
        {
            try
            {
                IsBusy = true;
                var listaCategoriasMovimientoGastos = await _servicioMovimiento.ObtenerCategoriasTipoGastosAsync();

                if (!CategoriaGastosIguales(listaCategoriasMovimientoGastos))
                {
                    ListaTipoCategoriasGastos?.Clear();
                    ListaTipoCategoriasGastos = new ObservableCollection<CategoriaMovimiento>(listaCategoriasMovimientoGastos);
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Error al cargar categorias de tipo gastos: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }
        private bool CategoriaGastosIguales(List<CategoriaMovimiento> categoriaMovimientosGasto)
        {
            if (ListaTipoCategoriasGastos?.Count != categoriaMovimientosGasto.Count) return false;

            return ListaTipoCategoriasGastos.Zip(categoriaMovimientosGasto, (actual, nuevo) =>
                actual.Id == nuevo.Id &&
                actual.Descripcion == nuevo.Descripcion &&
                actual.RutaIcono == nuevo.RutaIcono &&
                actual.TipoMovimiento == nuevo.TipoMovimiento)
                .All(iguales => iguales);

        }

        #endregion Metodo para cargar las categorias de gastos

        #region Metodo para cargar las categorias de ingresos

        private async Task CargarCategoriasIngresosAsync()
        {
            try
            {
                IsBusy = true;
                var listaCategoriasMovimientoIngresos = await _servicioMovimiento.ObtenerCategoriasTipoIngresosAsync();

                if (!CategoriaIngresosIguales(listaCategoriasMovimientoIngresos))
                {
                    ListaTipoCategoriasGastos?.Clear();
                    ListaTipoCategoriasGastos = new ObservableCollection<CategoriaMovimiento>(listaCategoriasMovimientoIngresos);
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Error al cargar categorias de tipo gastos: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }
        private bool CategoriaIngresosIguales(List<CategoriaMovimiento> categoriaIngresosNuevo)
        {
            if (ListaTipoCategoriasGastos?.Count != categoriaIngresosNuevo.Count) return false;

            return ListaTipoCategoriasGastos.Zip(categoriaIngresosNuevo, (actual, nuevo) =>
                actual.Id == nuevo.Id &&
                actual.Descripcion == nuevo.Descripcion &&
                actual.RutaIcono == nuevo.RutaIcono &&
                actual.TipoMovimiento == nuevo.TipoMovimiento)
                .All(iguales => iguales);
        }

        #endregion Metodo para cargar las categorias de gastos

        #region Metodo se encarga de seleccionar una categoría de gasto

        [RelayCommand]
        private async Task SeleccionarCategoriaGasto(CategoriaMovimiento categoriaGasto)
        {
            if (categoriaGasto is null) return;

            // Actualizas la categoría gasto seleccionada
            CategoriaSeleccionada = categoriaGasto;

            // Cierras el bottom sheet
            IsBottomSheetOpen = false;
        }

        #endregion Metodo se encarga de seleccionar una categoría de gasto

        #region Metodo para mostrar bottonsheet y actualizar la lista de categorias

        [RelayCommand]
        public async Task MostrarBottomSheetCategoriaGasto()
        {
            ListaCategoriasActual = ListaTipoCategoriasGastos;
            IsBottomSheetOpen = true;
        }

        #endregion Metodo para mostrar bottonsheet y actualizar la lista de categorias

        #region Metodo se encarga de seleccionar una categoría de ingreso

        [RelayCommand]
        private async Task SeleccionarCategoriaIngreso(CategoriaMovimiento categoriaIngreso)
        {
            if (categoriaIngreso is null) return;

            // Actualizas la categoría gasto seleccionada
            CategoriaSeleccionada = categoriaIngreso;

            // Cierras el bottom sheet
            IsBottomSheetOpen = false;
        }

        #endregion Metodo se encarga de seleccionar una categoría de ingreso

        #region Metodo para mostrar bottonsheet y actualizar la lista de categorias de ingreso

        [RelayCommand]
        public async Task MostrarBottomSheetCategoriaIngreso()
        {
            ListaCategoriasActual = ListaTipoCategoriasGastos;
            IsBottomSheetOpen = true;
        }

        #endregion Metodo para mostrar bottonsheet y actualizar la lista de categorias de ingreso

        #region Metodo para guardar el movimiento de Gasto

        [RelayCommand]
        public async Task GuardarMovimientoGasto()
        {
            //Validaciones antes de guardar el movimiento de gasto
            if (CategoriaSeleccionada is null) return;//Categoria seleccionada no puede estar vacia
            if (Monto <= 0 || Monto >= double.MaxValue) return; //El monto debe ser mayor a 0 o mayor a decimal.MaxValue
            if (string.IsNullOrWhiteSpace(Descripcion) || Descripcion.Length > 100) return; //La descripcion no puede estar vacia
            if (ListaTipoCategoriasGastos is null) return; //La lista de categorias no puede estar vacia
            if (Fecha > DateTime.Now || Fecha < DateTime.MinValue) return; //La fecha no puede ser mayor a la fecha actual o menor a DateTime.MinValue
            //if (string.IsNullOrWhiteSpace(Comercio)) return; //El comercio no puede estar vacio
            if (ListaTarjetas is null) return; //La lista de tarjetas no puede estar vacia
            if (TarjetaSeleccionada is null) return; //La tarjeta no puede estar vacia
            if (TarjetaSeleccionada.Tipo == "Credito")
            {
                if (EstaPagado == false)
                {
                    fondoColorBorder = "#E4E7FC";
                    fondoColorTexto = "#253FE4";
                }
                else
                {
                    fondoColorBorder = "#FED1E6";
                    fondoColorTexto = "#FA288A";
                }
            }
            Movimiento movimientoGasto = new Movimiento
            {
                TipoMovimiento = Enums.TipoMovimiento.Gasto.ToString(),
                Monto = Monto,
                Descripcion = this.Descripcion,
                FechaMovimiento = Fecha,
                CategoriaId = CategoriaSeleccionada.Id,
                TarjetaId = TarjetaSeleccionada.Id,
                EsPagado = this.EstaPagado,
                ColorFuenteEstado = fondoColorTexto,
                BorderFondoEstado = fondoColorBorder
            };
            //Esperamos el resultado de la operacion
            var resultado = await _servicioMovimiento.guardarMovimientoGasto(movimientoGasto);

            if (resultado > 0)
            {
                await Shell.Current.DisplayAlert("Éxito", "Movimiento guardado correctamente", "OK");
                LimpiarCampos();
            }
            else
            {
                await Shell.Current.DisplayAlert("Error", "No se pudo guardar el movimiento", "OK");
            }
        }

        #endregion Metodo para guardar el movimiento de Gasto

        #region Metodo para limpiar los campos despues de guardar

        private void LimpiarCampos()
        {
            Monto = 0;
            Descripcion = string.Empty;
            CategoriaSeleccionada = null;
            TarjetaSeleccionada = null;
            Descripcion = string.Empty;
            EstaPagado = false;
            fondoColorBorder = string.Empty;
            fondoColorTexto = string.Empty;
        }

        #endregion Metodo para limpiar los campos despues de guardar
    }
}