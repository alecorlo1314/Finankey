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
        public TipoCategoria? _tipoCategoriaGastoSeleccionada;
        [ObservableProperty]
        public bool _isBusy;
        [ObservableProperty]
        public bool _isBottomSheetOpen = false;
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
                new() { Descripcion = "Cosas del Hogar", Icono = "bticono_casa.svg" },
                new() { Descripcion = "Servicios publicos", Icono = "bticono_servicios.svg" },
                new() { Descripcion = "Internet y Telefonia", Icono = "btincono_internet.svg" },
                new() { Descripcion = "Mantenimiento", Icono = "bticono_mantenimiento.svg" },
                new() { Descripcion = "Streaming", Icono = "bticono_streaming.svg" },
                new() { Descripcion = "Cine", Icono = "bticono_cine.svg" },
                new() { Descripcion = "Medicinas", Icono = "btincono_medicinas.svg" },
                new() { Descripcion = "Medico", Icono = "bticono_medico.svg" },
                new() { Descripcion = "Libros", Icono = "bticono_libros.svg" },
                new() { Descripcion = "Cursos", Icono = "bticono_cursos.svg" }
            };
        }
        [RelayCommand]
        private async Task SeleccionarCategoriaGasto(TipoCategoria categoria)
        {
            if (categoria is null) return;

            // Actualizas la categoría seleccionada
            TipoCategoriaGastoSeleccionada = categoria;

            // Cierras el bottom sheet
            IsBottomSheetOpen = false;
        }

        [RelayCommand]
        public async Task MostrarBottomSheetCategoriaGasto()
        {
            IsBottomSheetOpen = true;
        }
    }
}
