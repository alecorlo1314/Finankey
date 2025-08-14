
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FinanKey.Models;
using FinanKey.Servicios;
using System.Collections.ObjectModel;

namespace FinanKey.ViewModels
{
    public partial class ViewModelIngreso : ObservableObject
    {
        #region Propiedades para Ingreso
        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(GuardarIngresoCommand))]
        private decimal _montoIngreso;
        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(GuardarIngresoCommand))]
        private string _descripcionIngreso = string.Empty;
        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(GuardarIngresoCommand))]
        private Categoria _categoriaIngresoSeleccionada;
        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(GuardarIngresoCommand))]
        private TipoCuenta _tipoCuentaIngresoSeleccionada;
        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(GuardarIngresoCommand))]
        private DateTime _fechaIngresoSeleccionada = DateTime.Now;
        #endregion

        // Propiedades para Ingreso
        public ObservableCollection<Categoria>? CategoriaIngreso { get; set; }
        public ObservableCollection<TipoCuenta>? TiposDeCuenta { get; set; }
        //inyeccion de dependencias
        private readonly IServiciosTransaccionIngreso _serviciosTransaccionIngreso;

        public ViewModelIngreso(IServiciosTransaccionIngreso serviciosTransaccionIngreso)
        {
            _serviciosTransaccionIngreso = serviciosTransaccionIngreso;

            // DATOS TIPO CUENTA
            TiposDeCuenta = new ObservableCollection<TipoCuenta>
                {
                    new TipoCuenta { Id=1,Descripcion = "Personal" },
                    new TipoCuenta { Id=2,Descripcion = "Ahorros" },
                    new TipoCuenta { Id=3,Descripcion = "Vacaciones" },
                    new TipoCuenta { Id=4,Descripcion = "Propiedad" }
                };

            // DATOS CATEGORIA GASTO
            CategoriaIngreso = new ObservableCollection<Categoria>
                {
                 new() { Id = 0, Color = "Green", TipoCategoria = new TipoCategoria { Id = 0, Descripcion = "Salario" } },
                 new() { Id = 1, Color = "Yellow", TipoCategoria = new TipoCategoria { Id = 1, Descripcion = "Inversiones" } },
                 new() { Id = 2, Color = "Red", TipoCategoria = new TipoCategoria { Id = 2, Descripcion = "Prestamo" } },
                };
        }

        [RelayCommand]
        private async Task GuardarIngreso()
        {
            Ingreso ingreso = new Ingreso
            {
                Monto = MontoIngreso,
                Descripcion = DescripcionIngreso,
                CategoriaId = CategoriaIngresoSeleccionada.Id,
                CuentaId = TipoCuentaIngresoSeleccionada.Id,
                Fecha = FechaIngresoSeleccionada
            };
            // Llamada al servicio para guardar la transacción
            bool resultado = await _serviciosTransaccionIngreso.CrearTransaccionIngresoAsync(ingreso);
            if (resultado)
            {
                await Shell.Current.DisplayAlert("Éxito", "Transacción de ingreso guardada correctamente.", "OK");
                MontoIngreso = 0;
                DescripcionIngreso = string.Empty;
                CategoriaIngreso = null;
                TipoCuentaIngresoSeleccionada = null;
                FechaIngresoSeleccionada = DateTime.Today;
            }
        }
    }
}
