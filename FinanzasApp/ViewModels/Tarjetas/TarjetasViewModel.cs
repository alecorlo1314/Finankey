using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FinanzasApp.Aplicacion.DTOs;
using FinanzasApp.Aplicacion.Interfaces;
using FinanzasApp.Aplicacion.Tarjetas.Comandos;
using FinanzasApp.Aplicacion.Tarjetas.Consultas;
using System.Collections.ObjectModel;

namespace FinanzasApp.Presentacion.ViewModels.Tarjetas;

public partial class TarjetasViewModel(IMediator mediador) : ViewModelBase
{
    #region 🧾 Estado de la UI

    // Lista de tarjetas mostradas en pantalla
    [ObservableProperty]
    private ObservableCollection<TarjetaResumenDto> _tarjetas = [];

    // Tarjeta seleccionada (opcional, para UI tipo carrusel)
    [ObservableProperty]
    private TarjetaResumenDto? _tarjetaSeleccionada;

    // Tarjeta seleccionada para eliminar (usada en el BottomSheet)
    [ObservableProperty]
    private TarjetaResumenDto? _tarjetaAEliminar;

    // Controla si el BottomSheet está abierto
    [ObservableProperty]
    private bool _mostrarSheetEliminar = false;

    // Indica si no hay tarjetas (estado vacío)
    [ObservableProperty]
    private bool _sinTarjetas;

    #endregion

    #region 🔄 Ciclo de vida

    public override async Task AlAparecerAsync()
    {
        // Reset del estado del BottomSheet
        MostrarSheetEliminar = false;
        TarjetaAEliminar = null;

        Titulo = "Mis Tarjetas";

        await CargarTarjetasAsync();
    }

    #endregion

    #region 🗑️ Flujo de eliminación (BottomSheet)

    /// Se ejecuta desde el Swipe → abre el BottomSheet
    [RelayCommand]
    private void MostrarConfirmacionEliminar(TarjetaResumenDto tarjeta)
    {
        TarjetaAEliminar = tarjeta;
        MostrarSheetEliminar = true;
    }

    /// Confirma eliminación desde el BottomSheet
    [RelayCommand]
    private async Task ConfirmarEliminarAsync()
    {
        MostrarSheetEliminar = false;

        if (TarjetaAEliminar is null) return;

        await EjecutarConCargaAsync(async () =>
        {
            await mediador.EnviarAsync(new EliminarTarjetaComando(TarjetaAEliminar.Id));

            TarjetaAEliminar = null;

            await CargarTarjetasAsync();
        });
    }

    /// Cancela eliminación (solo cierra el sheet)
    [RelayCommand]
    private void CancelarEliminar()
    {
        MostrarSheetEliminar = false;
        TarjetaAEliminar = null;
    }

    #endregion

    #region 📦 Carga de datos

    /// Carga o recarga las tarjetas desde BD
    [RelayCommand]
    private async Task CargarTarjetasAsync()
    {
        await EjecutarConCargaAsync(async () =>
        {
            //Paso 1: Obtener tarjetas
            var resultado = await mediador.ConsultarAsync(new ObtenerTarjetasConsulta());

            //Paso 2: Mapear
            var lista = resultado?.ToList() ?? new List<TarjetaResumenDto>();

            //Paso 3: Asignar los valores a la lista de tarjetas
            Tarjetas = new ObservableCollection<TarjetaResumenDto>(lista);

            //Paso 4: Indicar si no hay tarjetas y muestra el cartel
            SinTarjetas = !lista.Any();

            // Selección opcional
            TarjetaSeleccionada = lista.FirstOrDefault();
        });
    }

    #endregion

    #region 🧭 Navegación

    /// Navega a crear nueva tarjeta
    [RelayCommand]
    private static async Task AgregarTarjetaAsync() =>
        await Shell.Current.GoToAsync("/tarjetas/nueva");

    /// Navega al detalle de la tarjeta
    [RelayCommand]
    private async Task VerDetalleTarjetaAsync(TarjetaResumenDto tarjeta)
    {
        if (tarjeta is null) return;

        try
        {
            await Shell.Current.GoToAsync(
                "///tarjetas/detalle",
                new Dictionary<string, object>
                {
                    ["TarjetaId"] = tarjeta.Id,
                    ["NombreTarjeta"] = tarjeta.Nombre,
                    ["TipoTarjeta"] = tarjeta.Tipo
                });
        }
        catch (Exception ex)
        {
            await MostrarAlertaAsync("Error", ex.Message);
        }
    }

    /// Navega a editar tarjeta
    [RelayCommand]
    private static async Task EditarTarjetaAsync(TarjetaResumenDto tarjeta) =>
        await Shell.Current.GoToAsync(
            "//tarjetas/editar",
            new Dictionary<string, object>
            {
                ["TarjetaId"] = tarjeta.Id
            }
        );

    #endregion

    #region ⚠️ Eliminación directa (con alerta)

    /// Eliminación con confirmación clásica (sin BottomSheet)
    [RelayCommand]
    private async Task EliminarTarjetaAsync(TarjetaResumenDto tarjeta)
    {
        var confirmar = await ConfirmarAsync(
            "Eliminar tarjeta",
            $"¿Eliminar '{tarjeta.Nombre}'? Se conservará el historial de transacciones.");

        if (!confirmar) return;

        await EjecutarConCargaAsync(async () =>
        {
            await mediador.EnviarAsync(new EliminarTarjetaComando(tarjeta.Id));

            await CargarTarjetasAsync();
        });
    }

    #endregion
}