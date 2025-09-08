

using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace FinanKey.Presentacion.ViewModels
{
    public class ViewModelGestionCategorias : ObservableObject
    {
        public ObservableCollection<String>? ListaIconos { get; set; } 
        public ViewModelGestionCategorias()
        {
            cargarIconos();
        }

        private void cargarIconos()
        {
            ListaIconos = new ObservableCollection<string>()
            {
                "icono_accesorio.svg",
                "icono_accesorios.svg",
                "icono_avion.svg",
                "icono_bebidas.svg",
                "icono_bus.svg",
                "icono_cafe.svg",
                "icono_carro.svg",
                "icono_casa.svg",
                "icono_cine.svg",
                "icono_corte.svg",
                "icono_curso.svg",
                "icono_deporte.svg",
                "icono_familia.svg",
                "icono_gasolina.svg",
                "icono_herramienta.svg",
                "icono_higiene.svg",
                "icono_hospital.svg",
                "icono_internet.svg",
                "icono_inversion.svg",
                "icono_libro.svg",
                "icono_luz.svg",
                "icono_mascota.svg",
                "icono_pastilla.svg",
                "icono_regalo.svg",
                "icono_restaurante.svg",
                "icono_salario.svg",
                "icono_ropa.svg",
                "icono_serie.svg",
                "icono_software.svg",
                "icono_supermercado.svg",
                "icono_trabajo.svg",
                "icono_videojuegos.svg",            
            };
        }
    }
}

