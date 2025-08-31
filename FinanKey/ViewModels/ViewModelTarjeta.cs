using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FinanKey.Models;
using FinanKey.Servicios;
using System.Collections.ObjectModel;

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
        private string? montoInicial;
        [ObservableProperty]
        private string? categoria;
        [ObservableProperty]
        private string? ultimosCuatroDigitos;
        [ObservableProperty]
        private string? banco;
        [ObservableProperty]
        private string? nota;
        [ObservableProperty]
        private string? vencimiento;
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
        //Inyeccion de dependencias para el servicio de base de datos
        private readonly IServicioTarjeta _servicioTarjeta;
        public ViewModelTarjeta(IServicioTarjeta servicioTarjeta)
        {
            _servicioTarjeta = servicioTarjeta;
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
            if (isChecked)
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
        [RelayCommand]
        public void AgregarTarjeta()
        {
            var nuevaTarjeta = new Tarjeta
            {
                Nombre = NombreTarjeta,
                Ultimos4Digitos = UltimosCuatroDigitos,
                Tipo = EsVisibleMonto ? "Debito" : "Credito",
                Banco = banco,
                Vencimiento = Vencimiento,
                LimiteCredito = string.IsNullOrEmpty(LimiteCredito) ? null : double.Parse(LimiteCredito),
                MontoInicial = string.IsNullOrEmpty(MontoInicial) ? null : double.Parse(MontoInicial),
                Categoria = Categoria,
                ColorHex1 = LinearColor1,
                ColorHex2 = LinearColor2,
                Logo = LogoTarjeta,
                Nota = Nota
            };
            var resultado = _servicioTarjeta.AgregarAsync(nuevaTarjeta);

            if (resultado.Result > 0)
            {
                App.Current.MainPage.DisplayAlert("Éxito", "Tarjeta agregada correctamente", "OK");
                LimpiarCampos();
            }
            else
            {
                App.Current.MainPage.DisplayAlert("Error", "No se pudo agregar la tarjeta", "OK");
            }
        }
        private void LimpiarCampos()
        {
            NombreTarjeta = string.Empty;
            UltimosCuatroDigitos = string.Empty;
            Banco = string.Empty;
            Vencimiento = string.Empty;
            LimiteCredito = string.Empty;
            MontoInicial = string.Empty;
            Categoria = string.Empty;
            Nota = string.Empty;
            LinearColor1 = "#3E298F";
            LinearColor2 = "#836EDB";
            LogoTarjeta = "icono_visa.svg";
        }
    }
}
