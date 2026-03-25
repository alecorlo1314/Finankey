using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FinanzasApp.Domain.Interfaces;

namespace FinanzasApp.Presentacion.ViewModels.Configuracion;

/// <summary>
/// ViewModel de la pantalla de configuración.
/// Gestiona preferencias del usuario incluyendo la configuración biométrica.
/// </summary>
public partial class ConfiguracionViewModel(IServicioBiometrico servicioBiometrico) : ViewModelBase
{
    // ── Propiedades observables ───────────────────────────────────────────────

    /// <summary>Indica si el dispositivo soporta biometría</summary>
    [ObservableProperty]
    private bool _biometriaDisponible;

    /// <summary>Refleja y controla si la biometría está habilitada en la app</summary>
    [ObservableProperty]
    private bool _biometriaHabilitada;

    /// <summary>Descripción del tipo de biometría disponible ("Face ID", "Huella digital")</summary>
    [ObservableProperty]
    private string _descripcionBiometria = "Biometría";

    [ObservableProperty]
    private string _versionApp = "1.0.0";

    // ── Ciclo de vida ─────────────────────────────────────────────────────────

    public override async Task AlAparecerAsync()
    {
        Titulo = "Configuración";
        await VerificarBiometriaAsync();

        // Lee el estado actual persistido en Preferences
        BiometriaHabilitada = servicioBiometrico.BiometriaHabilitada;
        VersionApp = AppInfo.VersionString;
    }

    // ── Comandos ──────────────────────────────────────────────────────────────

    /// <summary>
    /// Alterna el estado de la biometría.
    /// Al habilitarla, solicita autenticación inmediata para confirmar que funciona.
    /// </summary>
    [RelayCommand]
    private async Task ToggleBiometriaAsync()
    {
        if (!BiometriaHabilitada)
        {
            // Intenta deshabilitar directamente
            servicioBiometrico.BiometriaHabilitada = false;
            return;
        }

        // Para habilitar, primero verificar que funciona con una prueba real
        var resultado = await servicioBiometrico.AutenticarAsync(
            titulo: "Confirmar biometría",
            descripcion: "Verifica tu identidad para habilitar el acceso biométrico");

        if (resultado.Exitoso)
        {
            servicioBiometrico.BiometriaHabilitada = true;
            await MostrarAlertaAsync("Biometría habilitada",
                "A partir de ahora podrás acceder con tu huella o Face ID.");
        }
        else
        {
            // Revierte el toggle si la autenticación falló
            BiometriaHabilitada = false;
            if (resultado.MensajeError is not null)
                MostrarError(resultado.MensajeError);
        }
    }

    /// <summary>Limpia los datos locales tras confirmación (útil para cerrar sesión)</summary>
    [RelayCommand]
    private async Task LimpiarDatosAsync()
    {
        var confirmar = await ConfirmarAsync(
            "Limpiar datos",
            "¿Deseas eliminar todos los datos locales? Esta acción no se puede deshacer.");

        if (!confirmar) return;

        // En producción aquí se llama al repositorio para limpiar la BD
        await MostrarAlertaAsync("Datos eliminados", "Todos los datos locales han sido eliminados.");
    }

    // ── Métodos privados ──────────────────────────────────────────────────────

    private async Task VerificarBiometriaAsync()
    {
        BiometriaDisponible = await servicioBiometrico.EsDisponibleAsync();

        // Determina el nombre descriptivo según el dispositivo
        DescripcionBiometria = DeviceInfo.Platform == DevicePlatform.iOS
            ? "Face ID / Touch ID"
            : "Huella digital";
    }
}
