using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FinanKey.Models;
using FinanKey.Servicios;
using FinanKey.View;
using System.Collections.ObjectModel;

namespace FinanKey.ViewModels
{
    public partial class ViewModelCuenta : ObservableObject
    {
        #region Propiedades para Cuenta
        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(GuardarCuentaCommand))]
        private string _nombreCuenta;
        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(GuardarCuentaCommand))]
        private string _nombreEntidadFinanciera;
        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(GuardarCuentaCommand))]
        private float? _saldo;
        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(GuardarCuentaCommand))]
        private TipoCuenta _tipoCuentaSeleccionada;
        #endregion

        //Lista de tipo cuenta
        public ObservableCollection<TipoCuenta>? TipoCuenta { get; set; }

        //Inicializacion Inyeccion de Dependencias
        private readonly IServicioTransaccionCuenta _serviciosTransaccionCuenta;

        public ViewModelCuenta(IServicioTransaccionCuenta serviciosTransaccionCuenta)
        {
            //INICIALIZACION DE INYECCION DE DEPENDENCIAS
            _serviciosTransaccionCuenta = serviciosTransaccionCuenta;
            // DATOS TIPO CUENTA
            TipoCuenta = new ObservableCollection<TipoCuenta>
                {
                    new TipoCuenta { Id=1,Descripcion = "Personal" },
                    new TipoCuenta { Id=2,Descripcion = "Ahorros" },
                    new TipoCuenta { Id=3,Descripcion = "Vacaciones" },
                    new TipoCuenta { Id=4,Descripcion = "Propiedad" }
                };
        }
        [RelayCommand]
        public async Task GuardarCuenta()
        {
            var cuenta = new Cuenta
            {
                NombreCuenta = NombreCuenta,
                NombreEntidadFinanciera = NombreEntidadFinanciera,
                Saldo = Saldo ?? 0,
                IDTipoCuenta = TipoCuentaSeleccionada.Id
            };
            if (await _serviciosTransaccionCuenta.CrearCuentaAsync(cuenta))
            {
                // Resetear campos
                NombreCuenta = string.Empty;
                NombreEntidadFinanciera = string.Empty;
                Saldo = null;
            }
        }
    }
}
