using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FinanKey.Models;

namespace FinanKey.ViewModels
{
    public partial class ViewModelTarjeta : ObservableObject
    {
        //Propiedades utilizadas en el viewmodel
        [ObservableProperty]
        private string? nombreTarjeta;
        [ObservableProperty]
        private string? limiteCredito;
        [ObservableProperty]
        private string? ultimosCuatroDigitos;
        [ObservableProperty]
        private string? linearColor1;
        [ObservableProperty]
        private string? linearColor2;
        public ViewModelTarjeta()
        {
            _ = inicializarGradiente();
        }
        private async Task inicializarGradiente()
        {
            linearColor1 = "#3E298F";
            linearColor2 = "#836EDB";
        }
        [RelayCommand]
        public void ColorTarjetaSeleccionada(string colores)
        {
            var split = colores.Split('|');
            if (split.Length == 2)
            {
                LinearColor1 = split[0];
                LinearColor2 = split[1];
            }
        }
    }
}
