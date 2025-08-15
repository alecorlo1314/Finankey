
using CommunityToolkit.Mvvm.ComponentModel;
using FinanKey.Models;

namespace FinanKey.ViewModels
{
    public partial class ViewModelTarjeta : ObservableObject
    {
        public List<Enums.TipoTarjeta> ListaTipoTarjeta { get; set; }
        public Enums.TipoTarjeta TipoTarjetaSeleccionada { get; set; }
        
        public ViewModelTarjeta()
        {
            _= AgregarTarjetaViewModel();
        }

        public async Task AgregarTarjetaViewModel()
        {
            ListaTipoTarjeta = Enum.GetValues(typeof(Enums.TipoTarjeta))
                                   .Cast<Enums.TipoTarjeta>()
                                   .ToList();

            // Seleccionar Debito por defecto
            TipoTarjetaSeleccionada = Enums.TipoTarjeta.Debito;
        }
    }
}
