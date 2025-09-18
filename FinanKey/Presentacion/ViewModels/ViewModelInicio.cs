using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FinanKey.Dominio.Models;
using FinanKey.Presentacion.View;
using System.Collections.ObjectModel;
using FinanKey.Aplicacion.UseCases;

namespace FinanKey.Presentacion.ViewModels
{
    public partial class ViewModelInicio : ObservableObject
    {
        #region INYECCION DE DEPENDENCIAS
        /// <summary>
        /// INYECCION AL SERVICIO DE INICIO DONDE SE REALIZAN DIFERENTES CONSULTAS A LAS INTERFASES DE TARJETAS Y MOVIMIENTOS
        /// </summary>
        public readonly ServicioInicio _servicioInicio; 
        #endregion

        #region LISTAS Y COLECCIONES
        [ObservableProperty]
        private ObservableCollection<Tarjeta> _listaTarjetas = [];
        [ObservableProperty]
        private ObservableCollection<Movimiento> _listaMovimientos = [];
        #endregion

        #region PROPIEDADES DE LA UI
        [ObservableProperty]
        // Propiedad para mostrar el indicador de carga si esta en true
        public bool isBusy;
        [ObservableProperty]
        //Propiedad indica si hay tarjetas, si hay mustra la lista de tarjetas, sino muestra un imagen que no hay tarjetas
        public bool _hayTarjetas = false; 
        [ObservableProperty]
        //Propiedad indica si no hay tarjetas, si no hay muestra un imagen que no hay tarjetas
        public bool _noHayMovimientos = true;
        [ObservableProperty]
        //Propiedad indica si hay movimientos, si hay mustra la lista de movimientos
        private bool _hayMovimientos = true;


        [ObservableProperty]
        public string _borderFondoEstado;
        [ObservableProperty]
        public string _colorFuenteEstado;
        #endregion

        #region VARIABLES ENVIADAS ENTRE PAGINAS
        [ObservableProperty]
        Tarjeta tarjeta;
        #endregion

        #region METODO CONSTRUCTOR
        public ViewModelInicio(ServicioInicio servicioInicio)
        {
            //Asignar los servicios a las variables privadas
            _servicioInicio = servicioInicio;
        }
        #endregion

        #region METODOS DE INICIALIZACION DE DATOS
        /// <summary>
        /// Metodo para cargar las tarjetas
        /// </summary>
        /// <returns></returns>
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
        /// <summary>
        /// Metodo para cargar los movimientos
        /// </summary>
        /// <returns></returns>
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
        #endregion

        #region METODOS DE COMPARACION DE DATOS
        /// <summary>
        /// Metodo sirve para comparar si los movimientos son iguales, solo se utiliza si se actualiza la lista de movimientos 
        /// con el fin de aumentar el rendimiento de la aplicacion y no recargar la lista si no hay cambios
        /// </summary>
        /// <param name="movimientosNuevos"></param>
        /// <returns></returns>
        private bool MovimientosIguales(List<Movimiento> movimientosNuevos)
        {
            // Si la cantidad de movimientos es diferente, no son iguales y se debe recargar
            if (ListaMovimientos.Count != movimientosNuevos.Count) return false;
            //Se realiza la comparacion de los movimientos actuales con los nuevos
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
        /// <summary>
        /// Metodo sirve para comparar si las tarjetas son iguales, solo se utiliza si se actualiza la lista de tarjetas
        /// con el fin de aumentar el rendimiento de la aplicacion y no recargar la lista si no hay cambios
        /// </summary>
        /// <param name="tarjetasNuevas"></param>
        /// <returns></returns>
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
        #endregion

        #region METODOS DE NAVEGACION
        /// <summary>
        /// Comando para navegar a la pagina de detalle de la tarjeta
        /// y se envia un parametros de Tipo Tarjeta
        /// </summary>
        /// <param name="tarjeta"></param>
        /// <returns></returns>
        [RelayCommand]
        public async Task NavegarADetalleTarjeta(Tarjeta tarjeta)
        {
            if (tarjeta is null)
                return;

            Tarjeta = tarjeta;

            await Shell.Current.GoToAsync(
                nameof(DetalleTarjetaPage),
                new Dictionary<string, object>
                {
                    [nameof(Tarjeta)] = Tarjeta
                });
        }
        /// <summary>
        /// Comando para navegar a la pagina de agregar tarjeta
        /// </summary>
        /// <returns></returns>
        [RelayCommand]
        public async Task NavegarAgregarTarjeta()
        {
            await Shell.Current.GoToAsync(nameof(AgregarTarjetaPage));
        }
        #endregion
    }
}
