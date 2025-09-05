using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FinanKey.Dominio.Models;
using FinanKey.Dominio.Interfaces;
using FinanKey.Presentacion.View;
using System.Collections.ObjectModel;
using FinanKey.Presentacion.View.Controles;

namespace FinanKey.Presentacion.ViewModels
{
    public partial class ViewModelInicio : ObservableObject
    {
        //Inyección de dependencias 
        public readonly IServicioTarjeta _servicioTarjeta;
        private readonly TabBarView _tabBarView;
        //Inicializar la lista de cuentas como una colección observable
        [ObservableProperty]
        private ObservableCollection<Tarjeta> _listaTarjetas = new();
        //Inicializacion de las propiedades usadas para la vista
        [ObservableProperty]
        private bool isBusy;
        [ObservableProperty]
        private bool _hayTarjetas = false;
        [ObservableProperty]
        private bool _hayMovimientos = true;

        public ViewModelInicio(IServicioTarjeta servicioTarjeta, TabBarView tabBarView)
        {
            //Asignar los servicios a las variables privadas
            _servicioTarjeta = servicioTarjeta;
            _tabBarView = tabBarView;
        }
        //Metodo para cargar las cuentas desde la base de datos
        public async Task CargarTarjetasAsync()
        {
            try
            {
                var tarjetas = await _servicioTarjeta.ObtenerTarjetasAsync();

                if (tarjetas != null && tarjetas.Count > 0)
                {
                    ListaTarjetas = new ObservableCollection<Tarjeta>(tarjetas);
                    HayTarjetas = !true;
                }
                else
                {
                    ListaTarjetas = new ObservableCollection<Tarjeta>();
                    HayTarjetas = !false;
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Error al cargar tarjetas: {ex.Message}", "OK");
            }
        }
        // // Comando para navegar a la página de detalle de cuenta con la cuenta seleccionada
        //[RelayCommand]
        //async Task NavegarADetalleCuenta(Cuenta cuenta)
        //{
        //    if (cuenta is null)
        //        return;

        //    cuentaSeleccionada = cuenta;

        //    await Shell.Current.GoToAsync(
        //        nameof(DetalleCuentaPage),
        //        new Dictionary<string, object>
        //        {
        //            ["Cuenta"] = cuentaSeleccionada
        //        });
        //}
        [RelayCommand]
        public async Task NavegarAgregarTarjeta()
        {
            await Shell.Current.GoToAsync(nameof(AgregarTarjetaPage));
        }
    }
}
