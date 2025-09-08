using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FinanKey.Dominio.Models;
using System.Collections.ObjectModel;

namespace FinanKey.Presentacion.ViewModels
{
    public partial class ViewModelGestionCategorias : ObservableObject
    {
        //ZONA DE LISTAS
        [ObservableProperty]
        public ObservableCollection<CategoriaMovimiento>? _listaCategorias;
        //ZONA DE ATRIBUTOS
        [ObservableProperty]
        public bool _isBottomSheetOpen;

        [ObservableProperty]
        public string? _descripcionCategoria;

        [ObservableProperty]
        public string? _rutaIcono;

        [ObservableProperty]
        public Icono? _iconoSeleccionado;

        [ObservableProperty]
        private string tipoMovimiento = "Ingreso"; // Valor inicial
        [ObservableProperty]
        public bool _noHayCategoriasGastos = true;
        [ObservableProperty]
        public bool _HayCategoriasIngresos = false;
   
        public ViewModelGestionCategorias()
        {
            cargarIconos();
        }

        public ObservableCollection<Icono>? ListaIconos { get; set; }

        [RelayCommand]
        public Task MostrarBottonShellAnadirCategoria()
        {
            IsBottomSheetOpen = true;
            return Task.CompletedTask;
        }
        private void cargarIconos()
        {
            ListaIconos = new ObservableCollection<Icono>()
            {
                new Icono { Id = 1, Nombre = "Accesorio", Ruta = "icono_accesorio.svg" },
                new Icono { Id = 2, Nombre = "Avión", Ruta = "icono_avion.svg" },
                new Icono { Id = 3, Nombre = "Bebidas", Ruta = "icono_bebida.svg" },
                new Icono { Id = 4, Nombre = "Bus", Ruta = "icono_bus.svg" },
                new Icono { Id = 5, Nombre = "Café", Ruta = "icono_cafe.svg" },
                new Icono { Id = 6, Nombre = "Carro", Ruta = "icono_carro.svg" },
                new Icono { Id = 7, Nombre = "Casa", Ruta = "icono_casa.svg" },
                new Icono { Id = 8, Nombre = "Cine", Ruta = "icono_cine.svg" },
                new Icono { Id = 9, Nombre = "Corte", Ruta = "icono_corte.svg" },
                new Icono { Id = 10, Nombre = "Curso", Ruta = "icono_curso.svg" },
                new Icono { Id = 11, Nombre = "Deporte", Ruta = "icono_deporte.svg" },
                new Icono { Id = 12, Nombre = "Familia", Ruta = "icono_familia.svg" },
                new Icono { Id = 13, Nombre = "Gasolina", Ruta = "icono_gasolina.svg" },
                new Icono { Id = 14, Nombre = "Herramienta", Ruta = "icono_herramienta.svg" },
                new Icono { Id = 15, Nombre = "Higiene", Ruta = "icono_higiene.svg" },
                new Icono { Id = 16, Nombre = "Hospital", Ruta = "icono_hospital.svg" },
                new Icono { Id = 17, Nombre = "Internet", Ruta = "icono_internet.svg" },
                new Icono { Id = 18, Nombre = "Inversión", Ruta = "icono_inversion.svg" },
                new Icono { Id = 19, Nombre = "Libro", Ruta = "icono_libro.svg" },
                new Icono { Id = 20, Nombre = "Luz", Ruta = "icono_luz.svg" },
                new Icono { Id = 21, Nombre = "Mascota", Ruta = "icono_mascota.svg" },
                new Icono { Id = 22, Nombre = "Pastilla", Ruta = "icono_pastilla.svg" },
                new Icono { Id = 23, Nombre = "Regalo", Ruta = "icono_regalo.svg" },
                new Icono { Id = 24, Nombre = "Restaurante", Ruta = "icono_restaurante.svg" },
                new Icono { Id = 25, Nombre = "Salario", Ruta = "icono_salario.svg" },
                new Icono { Id = 26, Nombre = "Ropa", Ruta = "icono_ropa.svg" },
                new Icono { Id = 27, Nombre = "Serie", Ruta = "icono_serie.svg" },
                new Icono { Id = 28, Nombre = "Software", Ruta = "icono_software.svg" },
                new Icono { Id = 29, Nombre = "Supermercado", Ruta = "icono_supermercado.svg" },
                new Icono { Id = 30, Nombre = "Trabajo", Ruta = "icono_trabajo.svg" },
                new Icono { Id = 31, Nombre = "Videojuegos", Ruta = "icono_videojuegos.svg" },
            };
        }

        [RelayCommand]
        public async Task GuardarCategoria()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(DescripcionCategoria) || IconoSeleccionado == null)
                {
                    await Shell.Current.DisplayAlert("Alerta", "Por favor, complete todos los campos.", "Aceptar");
                    return;
                }
                CategoriaMovimiento categoriaMovimiento = new()
                {
                    Descripcion = DescripcionCategoria,
                    Icon_id = IconoSeleccionado.Id,
                    RutaIcono = IconoSeleccionado.Ruta,
                    TipoMovimiento = TipoMovimiento
                };
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
            }
        }
    }
}   