using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FinanKey.Dominio.Models;
using System.Collections.ObjectModel;

namespace FinanKey.Presentacion.ViewModels
{
    public partial class ViewModelGestionCategorias : ObservableObject
    {
        [ObservableProperty]
        public bool _isBottomSheetOpen;

        [ObservableProperty]
        public string? _nombreCategoria;

        [ObservableProperty]
        public string? _rutaIcono;

        [ObservableProperty]
        public Icono? _iconoSeleccionado;

        [ObservableProperty]
        public Enums.TipoMovimiento _tipoMovimiento;

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
                new Icono { Nombre = "Accesorio", Ruta = "icono_accesorio.svg" },
                new Icono { Nombre = "Avión", Ruta = "icono_avion.svg" },
                new Icono { Nombre = "Bebidas", Ruta = "icono_bebida.svg" },
                new Icono { Nombre = "Bus", Ruta = "icono_bus.svg" },
                new Icono { Nombre = "Café", Ruta = "icono_cafe.svg" },
                new Icono { Nombre = "Carro", Ruta = "icono_carro.svg" },
                new Icono { Nombre = "Casa", Ruta = "icono_casa.svg" },
                new Icono { Nombre = "Cine", Ruta = "icono_cine.svg" },
                new Icono { Nombre = "Corte", Ruta = "icono_corte.svg" },
                new Icono { Nombre = "Curso", Ruta = "icono_curso.svg" },
                new Icono { Nombre = "Deporte", Ruta = "icono_deporte.svg" },
                new Icono { Nombre = "Familia", Ruta = "icono_familia.svg" },
                new Icono { Nombre = "Gasolina", Ruta = "icono_gasolina.svg" },
                new Icono { Nombre = "Herramienta", Ruta = "icono_herramienta.svg" },
                new Icono { Nombre = "Higiene", Ruta = "icono_higiene.svg" },
                new Icono { Nombre = "Hospital", Ruta = "icono_hospital.svg" },
                new Icono { Nombre = "Internet", Ruta = "icono_internet.svg" },
                new Icono { Nombre = "Inversión", Ruta = "icono_inversion.svg" },
                new Icono { Nombre = "Libro", Ruta = "icono_libro.svg" },
                new Icono { Nombre = "Luz", Ruta = "icono_luz.svg" },
                new Icono { Nombre = "Mascota", Ruta = "icono_mascota.svg" },
                new Icono { Nombre = "Pastilla", Ruta = "icono_pastilla.svg" },
                new Icono { Nombre = "Regalo", Ruta = "icono_regalo.svg" },
                new Icono { Nombre = "Restaurante", Ruta = "icono_restaurante.svg" },
                new Icono { Nombre = "Salario", Ruta = "icono_salario.svg" },
                new Icono { Nombre = "Ropa", Ruta = "icono_ropa.svg" },
                new Icono { Nombre = "Serie", Ruta = "icono_serie.svg" },
                new Icono { Nombre = "Software", Ruta = "icono_software.svg" },
                new Icono { Nombre = "Supermercado", Ruta = "icono_supermercado.svg" },
                new Icono { Nombre = "Trabajo", Ruta = "icono_trabajo.svg" },
                new Icono { Nombre = "Videojuegos", Ruta = "icono_videojuegos.svg" },
            };
        }

        [RelayCommand]
        public async Task GuardarCategoria()
        {
            if (string.IsNullOrWhiteSpace(NombreCategoria) || IconoSeleccionado == null )
            {
                await Shell.Current.DisplayAlert("Alerta", "Por favor, complete todos los campos.", "Aceptar");
            }
            
        }
    }
}