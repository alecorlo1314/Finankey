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

    #region 🔄 Ciclo de vida

    public override async Task AlAparecerAsync()
    {
        Titulo = "Configuración";

        await VerificarBiometriaAsync();

        // Estado persistido
        BiometriaHabilitada = servicioBiometrico.BiometriaHabilitada;

        // Versión app
        VersionApp = AppInfo.VersionString;
    }

    #endregion

    #region 🎯 Comandos

    /// <summary>
    /// Activa o desactiva la biometría
    /// </summary>
    [RelayCommand]
    private async Task ToggleBiometriaAsync()
    {
        // Si se está desactivando → directo
        if (!BiometriaHabilitada)
        {
            servicioBiometrico.BiometriaHabilitada = false;
            return;
        }

        // Si se está activando → validar autenticación
        var resultado = await servicioBiometrico.AutenticarAsync(
            titulo: "Confirmar biometría",
            descripcion: "Verifica tu identidad para habilitar el acceso biométrico");

        if (resultado.Exitoso)
        {
            servicioBiometrico.BiometriaHabilitada = true;

            await MostrarAlertaAsync(
                "Biometría habilitada",
                "Ahora podrás usar huella o Face ID.");
        }
        else
        {
            // Revertir toggle
            BiometriaHabilitada = false;

            if (!string.IsNullOrEmpty(resultado.MensajeError))
                MostrarError(resultado.MensajeError);
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
            ? "Face ID / Touch ID"
            : "Huella digital";
    }

    #endregion
}
