using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FinanKey.Dominio.Models;
using FinanKey.Dominio.Interfaces;
using FinanKey.Presentacion.View;
using System.Collections.ObjectModel;
using FinanKey.Aplicacion.UseCases;

namespace FinanKey.Presentacion.ViewModels
{
    public partial class ViewModelInicio : ObservableObject
    {
        //Inyección de dependencias 
        public readonly ServicioInicio _servicioInicio;
        //Listas Observable
        [ObservableProperty]
        private ObservableCollection<Tarjeta> _listaTarjetas = new();
        [ObservableProperty]
        private ObservableCollection<Movimiento> _listaMovimientos = new();
        //Inicializacion de las propiedades usadas para la vista
        [ObservableProperty]
        public bool isBusy;
        [ObservableProperty]
        public bool _hayTarjetas = false;
        [ObservableProperty]
        public bool _noHayMovimientos = true;
        [ObservableProperty]
        private bool _hayMovimientos = true;
        [ObservableProperty]
        public string _borderFondoEstado;
        [ObservableProperty]
        public string _colorFuenteEstado;

        public ViewModelInicio(ServicioInicio servicioInicio)
        {
            //Asignar los servicios a las variables privadas
            _servicioInicio = servicioInicio;
        }
        //Metodo para cargar las cuentas desde la base de datos
        public async Task CargarTarjetasAsync()
        {
            try
            {
                IsBusy = true;
                var tarjetas = await _servicioInicio.ObtenerTarjetasAsync();

                // Solo actualizar si hay cambios
                if (!TarjetasIguales(tarjetas))
                {
                    ListaTarjetas.Clear();
                    ListaTarjetas = new ObservableCollection<Tarjeta>(tarjetas);
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Error al cargar tarjetas: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }
        public async Task CargarMovimientosAsync()
        {
            try
            {
                IsBusy = true;
                var movimientos = await _servicioInicio.ObtenerMovimientosAsync();

                // Solo actualizar si hay cambios
                if (!MovimientosIguales(movimientos))
                {
                    ListaMovimientos.Clear();
                    ListaMovimientos = new ObservableCollection<Movimiento>(movimientos);
                    HayMovimientos = ListaMovimientos.Count > 0;
                    NoHayMovimientos = !HayMovimientos;
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Error al cargar tarjetas: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private bool MovimientosIguales(List<Movimiento> movimientosNuevos)
        {
            if (ListaMovimientos.Count != movimientosNuevos.Count) return false;

            return ListaMovimientos.Zip(movimientosNuevos, (actual, nuevo) =>
                actual.Id == nuevo.Id &&
                actual.TipoMovimiento == nuevo.TipoMovimiento &&
                actual.Monto == nuevo.Monto &&
                actual.Descripcion == nuevo.Descripcion &&
                actual.FechaMovimiento == nuevo.FechaMovimiento &&
                actual.CategoriaId == nuevo.CategoriaId &&
                actual.TarjetaId == nuevo.TarjetaId &&
                actual.EsPagado == nuevo.EsPagado &&
                actual.FechaCreacion == nuevo.FechaCreacion &&
                BorderFondoEstado == BorderFondoEstado &&
                ColorFuenteEstado == ColorFuenteEstado
                ).All(iguales => iguales);
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
        // Metodo Solo recargar si realmente hay cambios
        private bool TarjetasIguales(List<Tarjeta> tarjetasNuevas)
        {
            if (ListaTarjetas.Count != tarjetasNuevas.Count) return false;

            return ListaTarjetas.Zip(tarjetasNuevas, (actual, nuevo) =>
                actual.Id == nuevo.Id &&
                actual.Nombre == nuevo.Nombre &&
                actual.Ultimos4Digitos == nuevo.Ultimos4Digitos &&
                actual.Tipo == nuevo.Tipo &&
                actual.Banco == nuevo.Banco &&
                actual.Vencimiento == nuevo.Vencimiento &&
                actual.LimiteCredito == nuevo.LimiteCredito &&
                actual.MontoInicial == nuevo.MontoInicial &&
                actual.Categoria == nuevo.Categoria &&
                actual.Logo == nuevo.Logo &&
                actual.Descripcion == nuevo.Descripcion &&
                actual.ColorHex1 == nuevo.ColorHex1 &&
                actual.ColorHex2 == nuevo.ColorHex2 &&
                actual.FechaCreacion == nuevo.FechaCreacion)
                .All(iguales => iguales);
        }
    }
}
