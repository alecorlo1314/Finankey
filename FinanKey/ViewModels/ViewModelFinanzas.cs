using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FinanKey.Models;
using FinanKey.Servicios;
using FinanKey.View;
using System.Collections.ObjectModel;

namespace FinanKey.ViewModels
{
    public partial class ViewModelFinanzas : ObservableObject
    {
        //Inyección de dependencias 
        private readonly IServicioTransaccionCuenta _servicioCuenta;
        private readonly IServiciosTransaccionGasto _servicioTransaccionGasto;
        private readonly IServiciosTransaccionIngreso _servicioTransaccionIngreso;


        //Inicializar la lista de cuentas como una colección observable
        [ObservableProperty]
        private ObservableCollection<Cuenta> listaCuentas = new();
        [ObservableProperty]
        private ObservableCollection<Categoria> categoriaIngreso = new();
        [ObservableProperty]
        private ObservableCollection<Categoria> categoriaGasto = new();
        [ObservableProperty]
        private ObservableCollection<TipoCuenta> tipoCuenta = new();
        [ObservableProperty]
        private ObservableCollection<Transacciones> transacciones = new();
        [ObservableProperty]
        private Cuenta cuentaSeleccionada;

        //inicializar propiedades
        [ObservableProperty]
        public bool _hayMovimiento;
        [ObservableProperty]
        private bool isBusy;
        private bool _hayCuentas;

        public ViewModelFinanzas(IServicioTransaccionCuenta servicioCuenta,
                                IServiciosTransaccionGasto servicioTransaccionGasto,
                                IServiciosTransaccionIngreso servicioTransaccionIngreso)
        {
            //Asignar los servicios a las variables privadas
            _servicioCuenta = servicioCuenta;
            _servicioTransaccionGasto = servicioTransaccionGasto;
            _servicioTransaccionIngreso = servicioTransaccionIngreso;

            // Inicializar datos estaticos
            InicializarDatosEstáticos();
            //Inizacializar datos   
            _ = InicializarAsync();
        }

        private void InicializarDatosEstáticos()
        {
            TipoCuenta = new ObservableCollection<TipoCuenta>
            {
                new() { Id = 1, Descripcion = "Personal" },
                new() { Id = 2, Descripcion = "Ahorros" },
                new() { Id = 3, Descripcion = "Vacaciones" },
                new() { Id = 4, Descripcion = "Propiedad" }
            };

            CategoriaIngreso = new ObservableCollection<Categoria>
            {
                new() { Id = 0, Color = "Green", tipoCategoria = new TipoCategoria { Id = 0, Descripcion = "Salario" } },
                new() { Id = 1, Color = "Yellow", tipoCategoria = new TipoCategoria { Id = 1, Descripcion = "Inversiones" } },
                new() { Id = 2, Color = "Red", tipoCategoria = new TipoCategoria { Id = 2, Descripcion = "Prestamo" } }
            };

            CategoriaGasto = new ObservableCollection<Categoria>
            {
                new() { Id = 0, Color = "Green", tipoCategoria = new TipoCategoria { Id = 0, Descripcion = "Alimentación" } },
                new() { Id = 1, Color = "Yellow", tipoCategoria = new TipoCategoria { Id = 1, Descripcion = "Transporte" } },
                new() { Id = 2, Color = "Red", tipoCategoria = new TipoCategoria { Id = 2, Descripcion = "Salud" } },
                new() { Id = 3, Color = "Blue", tipoCategoria = new TipoCategoria { Id = 3, Descripcion = "Educación" } },
                new() { Id = 4, Color = "Purple", tipoCategoria = new TipoCategoria { Id = 4, Descripcion = "Entretenimiento" } }
            };
        }

        public async Task InicializarAsync()
        {
            IsBusy = true;
            await CargarCuentasAsync();
            await CargarListaMovimientosAsync();
            IsBusy = false;
        }
        public async Task CargarCuentasAsync()
        {
            try
            {
                var cuentas = await _servicioCuenta.ObtenerCuentasAsync();

                if (cuentas != null && cuentas.Count > 0)
                {
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        ListaCuentas.Clear();
                        foreach (var cuenta in cuentas)
                        {
                            cuenta.TipoCuenta = tipoCuenta.FirstOrDefault(t => t.Id == cuenta.IDTipoCuenta);
                            ListaCuentas.Add(cuenta);
                        }
                        _hayCuentas = ListaCuentas.Count > 0;
                    });
                }
                else
                {
                    _hayCuentas = false;
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Error al cargar cuentas: {ex.Message}", "OK");
            }
        }
        private async Task CargarListaMovimientosAsync()
        {
            try
            {
                var ingresos = await _servicioTransaccionIngreso.ObtenerTransaccionesIngresoAsync();
                var gastos = await _servicioTransaccionGasto.ObtenerTransaccionesGastoAsync();

                var listaTemp = new List<Transacciones>();

                foreach (var ingreso in ingresos)
                {
                    var cuenta = ListaCuentas.FirstOrDefault(c => c.IDTipoCuenta == ingreso.CuentaId);
                    var categoria = categoriaIngreso.FirstOrDefault(c => c.Id == ingreso.CategoriaId);

                    cuenta!.TipoCuenta = tipoCuenta.FirstOrDefault(t => t.Id == cuenta.IDTipoCuenta);

                    listaTemp.Add(new Transacciones
                    {
                        Monto = ingreso.Monto,
                        Descripcion = ingreso.Descripcion,
                        Fecha = ingreso.Fecha,
                        TipoMovimiento = ingreso.Tipo,
                        Cuenta = cuenta,
                        TipoCuenta = cuenta.TipoCuenta?.Descripcion,
                        Categoria = categoria,
                        TipoCategoria = categoria?.tipoCategoria?.Descripcion,
                        ColorTransaccion = ingreso.ColorIngreso
                    });
                }

                foreach (var gasto in gastos)
                {
                    var cuenta = ListaCuentas.FirstOrDefault(c => c.IDTipoCuenta == gasto.CuentaId);
                    var categoria = categoriaGasto.FirstOrDefault(c => c.Id == gasto.CategoriaId);

                    cuenta!.TipoCuenta = tipoCuenta.FirstOrDefault(t => t.Id == cuenta.IDTipoCuenta);

                    listaTemp.Add(new Transacciones
                    {
                        Monto = gasto.Monto,
                        Descripcion = gasto.Descripcion,
                        Fecha = gasto.Fecha,
                        TipoMovimiento = gasto.Tipo,
                        Cuenta = cuenta,
                        TipoCuenta = cuenta.TipoCuenta?.Descripcion,
                        Categoria = categoria,
                        TipoCategoria = categoria?.tipoCategoria?.Descripcion,
                        ColorTransaccion = gasto.ColorGasto
                    });
                }

                var ordenadas = listaTemp.OrderByDescending(t => t.Fecha).ToList();

                MainThread.BeginInvokeOnMainThread(() =>
                {
                    Transacciones.Clear();
                    foreach (var t in ordenadas)
                        Transacciones.Add(t);
                    HayMovimiento = Transacciones.Count < 0;
                });
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Error al cargar transacciones: {ex.Message}", "OK");
            }
        }
        // // Comando para navegar a la página de detalle de cuenta con la cuenta seleccionada
        [RelayCommand]
        async Task NavegarADetalleCuenta(Cuenta cuenta)
        {
            if (cuenta is null)
                return;

            cuentaSeleccionada = cuenta;

            await Shell.Current.GoToAsync(
                nameof(DetalleCuentaPage),
                new Dictionary<string, object>
                {
                    ["Cuenta"] = cuentaSeleccionada
                });
        }
    }
}
