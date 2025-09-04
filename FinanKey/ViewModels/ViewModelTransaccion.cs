using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FinanKey.Models;
using System.Collections.ObjectModel;

namespace FinanKey.ViewModels
{
    public partial class ViewModelTransaccion : ObservableObject
    {
        [ObservableProperty]
        public ObservableCollection<TipoCategoria>? _listaTipoCategoriasGastos;
        [ObservableProperty]
        public ObservableCollection<TipoCategoria>? _listaTipoCategoriasIngresos;
        [ObservableProperty]
        public ObservableCollection<TipoCategoria>? _listaCategoriasActual;
        [ObservableProperty]
        public TipoCategoria? _categoriaSeleccionada;
        [ObservableProperty]
        public bool _isBusy;
        [ObservableProperty]
        public bool _isBottomSheetOpen;
        public ViewModelTransaccion()
        {
            inicializarDatos();
        }
        private void inicializarDatos()
        {
            _isBusy = true;
                cargarCategorias();
            _isBusy = false;
        }
        private void cargarCategorias()
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

        [RelayCommand]
        private async Task SeleccionarCategoriaGasto(TipoCategoria categoriaGasto)
        {
            if (categoriaGasto is null) return;

            // Actualizas la categoría gasto seleccionada
            CategoriaSeleccionada = categoriaGasto;

            // Cierras el bottom sheet
            IsBottomSheetOpen = false;
        }
        [RelayCommand]
        public async Task MostrarBottomSheetCategoriaGasto()
        {
            ListaCategoriasActual = ListaTipoCategoriasGastos;
            IsBottomSheetOpen = true;
        }
        [RelayCommand]
        private async Task SeleccionarCategoriaIngreso(TipoCategoria categoriaIngreso)
        {
            if (categoriaIngreso is null) return;

            // Actualizas la categoría gasto seleccionada
            CategoriaSeleccionada = categoriaIngreso;

            // Cierras el bottom sheet
            IsBottomSheetOpen = false;
        }
        [RelayCommand]
        public async Task MostrarBottomSheetCategoriaIngreso()
        {
            ListaCategoriasActual = ListaTipoCategoriasGastos;
            IsBottomSheetOpen = true;
        }
    }
}
