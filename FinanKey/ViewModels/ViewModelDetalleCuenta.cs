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
        // Bandera para verificar si hay movimientos
        [ObservableProperty]
        private bool hayMovimiento;
        // Colecciones
        [ObservableProperty]
        private ObservableCollection<Cuenta> listaCuentas = new();
        [ObservableProperty]
        private ObservableCollection<Categoria> categoriaIngreso = new();
        [ObservableProperty]
        private ObservableCollection<Categoria> categoriaGasto = new();
        [ObservableProperty]
        private ObservableCollection<TipoCuenta> tipoCuenta = new();
        public ViewModelDetalleCuenta(IServiciosTransaccionIngreso servicioTransaccionIngreso,
                                IServiciosTransaccionGasto servicioTransaccionGasto)
        {
            _servicioTransaccionGasto = servicioTransaccionGasto;
            _servicioTransaccionIngreso = servicioTransaccionIngreso;
            // Inicializar datos
            InicializarDatosEstáticos();
            _ = CargarListaMovimientosAsync();
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

        private async Task CargarListaMovimientosAsync()
        {
            try
            {
                IsBusy = true;
                var ingresos = await _servicioTransaccionIngreso.ObtenerTransaccionesIngresoPorCuentaAsync(Cuenta.Id);
                var gastos = await _servicioTransaccionGasto.ObtenerTransaccionesGastoPorCuentaAsync(Cuenta.Id);

                var listaTemp = new List<Transacciones>();

                //llenamos la lista de transacciones con los ingresos
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
            finally
            {
                IsBusy = false;
            }
        }
    }
}
