using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FinanzasApp.Aplicacion.DTOs;
using FinanzasApp.Aplicacion.Interfaces;
using FinanzasApp.Aplicacion.Tarjetas.Consultas;
using FinanzasApp.Aplicacion.Transacciones.Consultas;
using System.Collections.ObjectModel;

namespace FinanzasApp.Presentacion.ViewModels;

public partial class DashboardViewModel(IMediator mediador) : ViewModelBase
{
    #region 🧾 Estado de la UI

    // Resumen financiero del mes
    [ObservableProperty]
    private ResumenFinancieroDto? _resumenFinanciero;

    // Lista de transacciones recientes
    [ObservableProperty]
    private ObservableCollection<TransaccionResumenDto> _transaccionesRecientes = [];

    // Lista de tarjetas (limitadas para dashboard)
    [ObservableProperty]
    private ObservableCollection<TarjetaResumenDto> _tarjetas = [];

    // Saludo dinámico según la hora
    [ObservableProperty]
    private string _saludoUsuario = string.Empty;

    #endregion

    #region 📅 Filtros de fecha

    // Mes seleccionado (por defecto el actual)
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(EtiquetaMes))]
    private DateTime _mesFiltro = new(DateTime.Now.Year, DateTime.Now.Month, 1);

    // Fecha actual (puede usarse en UI)
    [ObservableProperty]
    private DateTime _fechaHoy = DateTime.Now;

    // Texto formateado del mes (ej: "marzo 2026")
    public string EtiquetaMes => MesFiltro.ToString("MMMM yyyy");

    #endregion

    #region 🔄 Ciclo de vida

    public override async Task AlAparecerAsync()
    {
        Titulo = "Inicio";

        ActualizarSaludo();

        await CargarDashboardAsync();
    }

    #endregion

    #region 🧭 Navegación

    /// Navega a crear nueva tarjeta
    [RelayCommand]
    private static async Task AgregarTarjetaAsync()
    {
        await Shell.Current.GoToAsync("/tarjetas/nueva");
    }

    #endregion

    #region 📦 Carga de datos

    /// Carga toda la información del dashboard en paralelo
    [RelayCommand]
    private async Task CargarDashboardAsync()
    {
        await EjecutarConCargaAsync(async () =>
        {
            // Ejecuta consultas en paralelo (mejor rendimiento)
            var tareaResumen = mediador.ConsultarAsync(
                new ObtenerResumenFinancieroConsulta(MesFiltro.Year, MesFiltro.Month));

            var tareaRecientes = mediador.ConsultarAsync(
                new ObtenerTransaccionesRecientesConsulta(10));

            var tareaTarjetas = mediador.ConsultarAsync(
                new ObtenerTarjetasConsulta());

            await Task.WhenAll(tareaResumen, tareaRecientes, tareaTarjetas);

            // Asignación de resultados
            ResumenFinanciero = await tareaResumen;

            Tarjetas = new ObservableCollection<TarjetaResumenDto>(
                (await tareaTarjetas).Take(5));

            TransaccionesRecientes = new ObservableCollection<TransaccionResumenDto>(
                await tareaRecientes);
        });
    }

    #endregion

    #region ⏳ Navegación entre meses

    /// Va al mes anterior
    [RelayCommand]
    private async Task MesAnteriorAsync()
    {
        MesFiltro = MesFiltro.AddMonths(-1);

        await CargarDashboardAsync();
    }

    /// Va al mes siguiente (no permite futuro)
    [RelayCommand]
    private async Task MesSiguienteAsync()
    {
        if (MesFiltro < new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1))
        {
            MesFiltro = MesFiltro.AddMonths(1);

            await CargarDashboardAsync();
        }
    }

    #endregion

    #region 🧠 Helpers

    /// Genera saludo dinámico según la hora
    private void ActualizarSaludo()
    {
        var hora = DateTime.Now.Hour;

        SaludoUsuario = hora switch
        {
            >= 5 and < 12 => "¡Buenos días! ☀️",
            >= 12 and < 19 => "¡Buenas tardes! 🌤️",
            _ => "¡Buenas noches! 🌙"
        };
    }

    #endregion
}