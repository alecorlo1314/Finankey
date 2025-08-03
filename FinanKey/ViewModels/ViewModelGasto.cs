using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FinanKey.Models;
using FinanKey.Servicios;
using System.Collections.ObjectModel;

namespace FinanKey.ViewModels
{
    public partial class ViewModelGasto : ObservableObject
    {
        #region Propiedades para Gasto
        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(GuardarGastoCommand))]
        private float _montoGasto;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(GuardarGastoCommand))]
        private string _descripcionGasto = string.Empty;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(GuardarGastoCommand))]
        private Categoria _categoriaGastoSeleccionado;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(GuardarGastoCommand))]
        private TipoCuenta _tipoCuentaSeleccionada;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(GuardarGastoCommand))]
        private DateTime _fechaSeleccionada = DateTime.Today;
        #endregion

        //Prueba para comboBox tipoCuenta y categoria
        public ObservableCollection<TipoCuenta>? TiposDeCuenta { get; set; }
        public ObservableCollection<Categoria>? CategoriaGasto { get; set; }

        //Inicializacion Inyeccion de Dependencias
        private readonly IServiciosTransaccionGasto _serviciosTransaccionGasto;

        public ViewModelGasto(IServiciosTransaccionGasto serviciosTransaccionGasto)
        {
            //INICIALIZACION DE INYECCION DE DEPENDENCIAS
            _serviciosTransaccionGasto = serviciosTransaccionGasto;
            //inicializar datos estaticos
            InicializarDatosEstáticos();
        }
        //cargar datos estaticos
        private void InicializarDatosEstáticos()
        {
            // DATOS TIPO CUENTA
            TiposDeCuenta = new ObservableCollection<TipoCuenta>
                {
                    new TipoCuenta { Id=1,Descripcion = "Personal" },
                    new TipoCuenta { Id=2,Descripcion = "Ahorros" },
                    new TipoCuenta { Id=3,Descripcion = "Vacaciones" },
                    new TipoCuenta { Id=4,Descripcion = "Propiedad" }
                };

            // DATOS CATEGORIA INGRESO
            CategoriaGasto = new ObservableCollection<Categoria>
            {
                new() { Id = 0, Color = "Green", tipoCategoria = new TipoCategoria { Id = 0, Descripcion = "Alimentacion" } },
                new() { Id = 1, Color = "Yellow", tipoCategoria = new TipoCategoria { Id = 1, Descripcion = "Transporte" } },
                new() { Id = 2, Color = "Red", tipoCategoria = new TipoCategoria { Id = 2, Descripcion = "Salud" } },
                new() { Id = 3, Color = "Blue", tipoCategoria = new TipoCategoria { Id = 3, Descripcion = "Educacion" } },
                new() { Id = 4, Color = "Purple", tipoCategoria = new TipoCategoria { Id = 4, Descripcion = "Entretenimiento" } }
            };
        }

        [RelayCommand]
        private async Task GuardarGasto()
        {
            // Validar si las listas de categorías y cuentas están vacías
            if (CategoriaGastoSeleccionado == null || TipoCuentaSeleccionada == null)
            {
                await Shell.Current.DisplayAlert("Error", "Debe seleccionar una categoría y una cuenta.", "OK");
                return;
            }

            Gasto gastoTransaccion = new Gasto
            {
                Monto = (decimal)MontoGasto,
                Descripcion = DescripcionGasto,
                CategoriaId = CategoriaGastoSeleccionado.Id,
                CuentaId = TipoCuentaSeleccionada.Id,
                Fecha = FechaSeleccionada
            };

            // Llamada al servicio para guardar la transacción
            bool resultado = await _serviciosTransaccionGasto.CrearTransaccionGastoAsync(gastoTransaccion);

            if (resultado)
            {
                await Shell.Current.DisplayAlert("Éxito", "Transacción de gasto guardada correctamente.", "OK");
                LimpiarCampos();
            }
        }

        private void LimpiarCampos()
        {
            MontoGasto = 0;
            DescripcionGasto = string.Empty;
            CategoriaGastoSeleccionado = null;
            TipoCuentaSeleccionada = null;
            FechaSeleccionada = DateTime.Today;
        }
    }
}