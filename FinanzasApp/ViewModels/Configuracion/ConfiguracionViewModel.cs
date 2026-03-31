using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FinanzasApp.Domain.Interfaces;

namespace FinanzasApp.Presentacion.ViewModels.Configuracion;

/// <summary>
/// ViewModel de la pantalla de configuración.
/// Maneja biometría, versión de app y acciones generales.
/// </summary>
public partial class ConfiguracionViewModel(IServicioBiometrico servicioBiometrico) : ViewModelBase
{
    #region 🧾 Propiedades observables

    /// <summary>Indica si el dispositivo soporta biometría</summary>
    [ObservableProperty]
    private bool _biometriaDisponible;

    /// <summary>Indica si la biometría está habilitada</summary>
    [ObservableProperty]
    private bool _biometriaHabilitada;

    /// <summary>Texto descriptivo del tipo de biometría</summary>
    [ObservableProperty]
    private string _descripcionBiometria = "Biometría";

    /// <summary>Versión de la app</summary>
    [ObservableProperty]
    private string _versionApp = "1.0.0";

    #endregion

    #region 🔒 Estado interno
    private bool _disponibilidadVerificada = false;
    private bool _sincronizandoEstado = false;

    #endregion

    #region 🔄 Ciclo de vida

    public override async Task AlAparecerAsync()
    {
        Titulo = "Ajustes";

        if (!_disponibilidadVerificada)
        {
            await VerificarBiometriaAsync();
            _disponibilidadVerificada = true;
        }

        SincronizarEstadoBiometria();

        VersionApp = AppInfo.VersionString;
    }

    #endregion

    #region 🎯 Comandos
    /// <summary>
    /// Recibe bool directamente desde el converter.
    /// El parámetro es el nuevo estado del switch.
    /// </summary>
    [RelayCommand]
    private async Task ToggleBiometriaAsync(bool nuevoEstado)
    {
        if (_sincronizandoEstado) return;

        if (nuevoEstado)
        {
            // Quiere habilitar → verificar que funcione con autenticación real
            var resultado = await servicioBiometrico.AutenticarAsync(
                titulo: "Confirmar biometría",
                descripcion: "Verifica tu identidad para habilitar el acceso biométrico");

            if (resultado.Cancelado)
            {
                // Revertir sin disparar otra vez el comando
                _sincronizandoEstado = true;
                BiometriaHabilitada = false;
                _sincronizandoEstado = false;
                return;
            }

            if (resultado.Exitoso)
            {
                servicioBiometrico.BiometriaHabilitada = true;
            }
            else
            {
                // Revertir el switch sin disparar autenticación
                _sincronizandoEstado = true;
                BiometriaHabilitada = false;
                _sincronizandoEstado = false;

                if (resultado.MensajeError is not null)
                    MostrarError(resultado.MensajeError);
            }
        }
        else
        {
            // Deshabilitar no requiere autenticación
            servicioBiometrico.BiometriaHabilitada = false;
        }
    }

    /// <summary>
    /// Limpia datos locales de la app
    /// </summary>
    [RelayCommand]
    private async Task LimpiarDatosAsync()
    {
        var confirmar = await ConfirmarAsync(
            "Limpiar datos",
            "¿Deseas eliminar todos los datos locales?");

        if (!confirmar) return;

        // Aquí iría lógica real de limpieza (BD, cache, etc.)
        await MostrarAlertaAsync(
            "Datos eliminados",
            "Todos los datos locales fueron eliminados.");
    }

    #endregion

    #region 🔍 Métodos privados

    /// <summary>
    /// Verifica si el dispositivo soporta biometría
    /// </summary>
    private async Task VerificarBiometriaAsync()
    {
        BiometriaDisponible = await servicioBiometrico.EsDisponibleAsync();

        // Define texto según plataforma
        DescripcionBiometria = DeviceInfo.Platform == DevicePlatform.iOS
            ? "Face ID"
            : "Huella digital";
    }

    /// <summary>
    /// Sincroniza el estado del toggle con lo que estaba guardado en Preferences
    /// </summary>
    private void SincronizarEstadoBiometria()
    {
        _sincronizandoEstado = true;
        BiometriaHabilitada = servicioBiometrico.BiometriaHabilitada;
        _sincronizandoEstado = false;
    }

    #endregion
}
