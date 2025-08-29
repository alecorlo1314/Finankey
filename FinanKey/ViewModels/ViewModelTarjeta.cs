using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FinanKey.Models;
using Syncfusion.Maui.Buttons;
using Syncfusion.Maui.Toolkit.BottomSheet;
using System.Collections.ObjectModel;
using Syncfusion.Maui.Buttons;

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
        [ObservableProperty]
        private string? logoTarjeta;
        [ObservableProperty]
        private bool esVisibleMonto = false;
        [ObservableProperty]
        private bool esVisibleLimiteCredito = true;
        //Lista de logos de tarjeta
        public ObservableCollection<OpcionTarjeta> ListaLogoTarjeta { get; set; }
        public ViewModelTarjeta()
        {
            _ = inicializarGradiente();
            _ = inicializarLogo();
        }
        private async Task inicializarGradiente()
        {
            linearColor1 = "#3E298F";
            linearColor2 = "#836EDB";
        }
        private async Task inicializarLogo()
        {
            LogoTarjeta = "icono_visa.svg";
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
        [RelayCommand]
        public void IconoSeleccionado(string icono)
        {
            LogoTarjeta = icono;
        }
        [RelayCommand]
        public void MostrarMontoInicial(bool isChecked)
        {
            if(isChecked)
            {
                EsVisibleLimiteCredito = false;
                EsVisibleMonto = true;
            }
        }
        [RelayCommand]
        public void MostrarLimiteTarjeta(bool isChecked)
        {
            if (isChecked)
            {
                EsVisibleLimiteCredito = true;
                EsVisibleMonto = false;
            }
        }
    }
}
