using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FinanKey.Dominio.Models;
using FinanKey.Aplicacion.UseCases;
using System.Collections.ObjectModel;

namespace FinanKey.Presentacion.ViewModels
{
    public partial class ViewModelMovimiento : ObservableObject
    {
        //Inyeccion de Dependencias
        private readonly ServicioMovimiento _servicioMovimiento;
        //Propiedades de Movimiento
        [ObservableProperty]
        public double _monto = 0;
        [ObservableProperty]
        public TipoCategoria? _categoriaSeleccionada;
        [ObservableProperty]
        public DateTime _fecha = DateTime.Now;
        [ObservableProperty]
        public string? _comercio;
        [ObservableProperty]
        public Tarjeta? _tarjetaSeleccionada;
        [ObservableProperty]
        public bool? _estaPagado;
        [ObservableProperty]
        public string? _descripcion;
        //Colecciones o listas
        [ObservableProperty]
        public ObservableCollection<TipoCategoria>? _listaTipoCategoriasGastos;
        [ObservableProperty]
        public ObservableCollection<TipoCategoria>? _listaTipoCategoriasIngresos;
        [ObservableProperty]
        public ObservableCollection<TipoCategoria>? _listaCategoriasActual;
        [ObservableProperty]
        public ObservableCollection<Tarjeta>? _listaTarjetas;
        //Para visar al inicializador de categorias cuando este listo
        [ObservableProperty]
        public bool _isBusy;
        //Para abrir y cerrar el bottom sheet
        [ObservableProperty]
        public bool _isBottomSheetOpen;
        public ViewModelMovimiento(ServicioMovimiento servicioMovimiento)
        {
            inicializarDatos();
            _servicioMovimiento = servicioMovimiento;
        }

        //Metodo para inicializar las categorias
        private void inicializarDatos()
        {
            _isBusy = true;
            cargarCategoriasGastos();
            _isBusy = false;
        }
        public async Task CargarTarjetasAsync()
        {
            var resultado = await _servicioMovimiento.obtenerTarjetas();
            if (resultado != null)
            {
                ListaTarjetas = new ObservableCollection<Tarjeta>(resultado);
            }
        }

        #region Metodo para cargar las categorias de gastos
        private void cargarCategoriasGastos()
        {
            ListaTipoCategoriasGastos = new ObservableCollection<TipoCategoria>
            {
                new() {Id = 0, Descripcion = "Cosas del Hogar", Icono = "bticono_casa.svg" },
                new() {Id = 1, Descripcion = "Servicios publicos", Icono = "bticono_servicios.svg" },
                new() {Id = 2, Descripcion = "Internet y Telefonia", Icono = "btincono_internet.svg" },
                new() {Id = 3, Descripcion = "Mantenimiento", Icono = "bticono_mantenimiento.svg" },
                new() {Id = 4, Descripcion = "Streaming", Icono = "bticono_streaming.svg" },
                new() {Id = 5, Descripcion = "Cine", Icono = "bticono_cine.svg" },
                new() {Id = 6, Descripcion = "Medicinas", Icono = "btincono_medicinas.svg" },
                new() {Id = 7, Descripcion = "Medico", Icono = "bticono_medico.svg" },
                new() {Id = 8, Descripcion = "Libros", Icono = "bticono_libros.svg" },
                new() {Id = 9, Descripcion = "Cursos", Icono = "bticono_cursos.svg" }
            };
        }
        #endregion

        #region Metodo se encarga de seleccionar una categoría de gasto
        [RelayCommand]
        private async Task SeleccionarCategoriaGasto(TipoCategoria categoriaGasto)
        {
            if (categoriaGasto is null) return;

            // Actualizas la categoría gasto seleccionada
            CategoriaSeleccionada = categoriaGasto;

            // Cierras el bottom sheet
            IsBottomSheetOpen = false;
        }
        #endregion

        #region Metodo para mostrar bottonsheet y actualizar la lista de categorias
        [RelayCommand]
        public async Task MostrarBottomSheetCategoriaGasto()
        {
            ListaCategoriasActual = ListaTipoCategoriasGastos;
            IsBottomSheetOpen = true;
        }
        #endregion

        #region Metodo se encarga de seleccionar una categoría de ingreso
        [RelayCommand]
        private async Task SeleccionarCategoriaIngreso(TipoCategoria categoriaIngreso)
        {
            if (categoriaIngreso is null) return;

            // Actualizas la categoría gasto seleccionada
            CategoriaSeleccionada = categoriaIngreso;

            // Cierras el bottom sheet
            IsBottomSheetOpen = false;
        }
        #endregion

        #region Metodo para mostrar bottonsheet y actualizar la lista de categorias de ingreso
        [RelayCommand]
        public async Task MostrarBottomSheetCategoriaIngreso()
        {
            ListaCategoriasActual = ListaTipoCategoriasGastos;
            IsBottomSheetOpen = true;
        }
        #endregion

        #region Metodo para guardar el movimiento de Gasto
        [RelayCommand]
        public async Task GuardarMovimientoGasto()
        {
            //Validaciones antes de guardar el movimiento de gasto
            if (CategoriaSeleccionada is null) return;//Categoria seleccionada no puede estar vacia
            if (Monto <= 0 || Monto >= double.MaxValue) return; //El monto debe ser mayor a 0 o mayor a decimal.MaxValue
            if(string.IsNullOrWhiteSpace(Descripcion) || Descripcion.Length > 100) return; //La descripcion no puede estar vacia
            if (ListaTipoCategoriasGastos is null) return; //La lista de categorias no puede estar vacia
            if(Fecha > DateTime.Now || Fecha < DateTime.MinValue) return; //La fecha no puede ser mayor a la fecha actual o menor a DateTime.MinValue
            if (string.IsNullOrWhiteSpace(Comercio)) return; //El comercio no puede estar vacio
            if(ListaTarjetas is null) return; //La lista de tarjetas no puede estar vacia
            if(TarjetaSeleccionada is null) return; //La tarjeta no puede estar vacia

            Movimiento movimientoGasto = new Movimiento
            {
                TipoMovimiento = Enums.TipoMovimiento.Gasto.ToString(),
                Monto = Monto,
                Descripcion = this.Descripcion,
                Fecha = Fecha,
                CategoriaId = CategoriaSeleccionada.Id,
                Comercio = this.Comercio,
                TarjetaId = TarjetaSeleccionada.Id,
                EsPagado = this.EstaPagado,
            };
            //Esperamos el resultado de la operacion
            var resultado = await _servicioMovimiento.guardarMovimientoGasto(movimientoGasto);

            if(resultado > 0)
            {
                await Shell.Current.DisplayAlert("Éxito", "Movimiento guardado correctamente", "OK");
                LimpiarCampos();
            }
            else
            {
                await Shell.Current.DisplayAlert("Error", "No se pudo guardar el movimiento", "OK");
            }
        }
        #endregion

        #region Metodo para limpiar los campos despues de guardar
        private void LimpiarCampos()
        {
            Monto = 0;
            Descripcion = string.Empty;
            CategoriaSeleccionada = null;
            Comercio = string.Empty;
            TarjetaSeleccionada = null;
            EstaPagado = false;
        }
        #endregion  
    }
}
