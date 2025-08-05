using CommunityToolkit.Mvvm.ComponentModel;
using FinanKey.Models;
using FinanKey.Servicios;
using System.Collections.ObjectModel;

namespace FinanKey.ViewModels
{
    // Recibe los datos de la cuenta seleccionada desde la vista DetalleCuentaPage.xaml.cs
    [QueryProperty(nameof(Cuenta), nameof(Cuenta))]
    [QueryProperty(nameof(Transacciones), nameof(Transacciones))]
    public partial class ViewModelDetalleCuenta : ObservableObject
    {
        // Inyección de dependencias
        private readonly IServiciosTransaccionGasto _servicioTransaccionGasto;
        private readonly IServiciosTransaccionIngreso _servicioTransaccionIngreso;
        // Propiedades para recibir los datos de la cuenta y las transacciones
        [ObservableProperty]
        private Cuenta cuenta = new();
        [ObservableProperty]
        private ObservableCollection<Transacciones> transacciones = new();
        //Estado de ocupado
        [ObservableProperty]
        private bool isBusy;
        public ViewModelDetalleCuenta(IServiciosTransaccionIngreso servicioTransaccionIngreso,
                                IServiciosTransaccionGasto servicioTransaccionGasto)
        {
            _servicioTransaccionGasto = servicioTransaccionGasto;
            _servicioTransaccionIngreso = servicioTransaccionIngreso;
            // Inicializar datos
            _ = InicializarAsync();
        }

        private async Task InicializarAsync()
        {
            isBusy = true;
            try
            {
                
            }
            catch
            {

            }finally
            {
                isBusy = false;
            }
        }
    }
}
