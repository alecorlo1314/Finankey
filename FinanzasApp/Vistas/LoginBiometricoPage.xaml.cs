using FinanzasApp.Domain.Interfaces;

namespace FinanzasApp.Presentacion.Vistas;

/// <summary>
/// Página de autenticación biométrica que se muestra al abrir la app
/// si el usuario tiene la biometría habilitada en configuración.
/// 
/// FLUJO:
/// 1. App abre → verifica si biometría está habilitada
/// 2. Si sí: muestra esta página y solicita autenticación
/// 3. Si autenticación exitosa: navega al Shell principal
/// 4. Si falla 3 veces: ofrece opción de PIN o reintentar
/// </summary>
public partial class LoginBiometricoPage : ContentPage
{
    private readonly IServicioBiometrico _servicioBiometrico;
    private int _intentosFallidos = 0;
    private const int MaxIntentos = 3;

    public LoginBiometricoPage(IServicioBiometrico servicioBiometrico)
    {
        InitializeComponent();
        _servicioBiometrico = servicioBiometrico;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        // Si la biometría está habilitada, solicitar autenticación
        await SolicitarAutenticacionAsync();
    }

    /// <summary>
    /// Solicita autenticación biométrica. Si falla repetidamente,
    /// desbloquea la opción de entrar sin biometría.
    /// </summary>
    private async Task SolicitarAutenticacionAsync()
    {
        //Paso 1: Realizar la autenticación de la biometría
        var resultado = await _servicioBiometrico.AutenticarAsync(
            titulo: "FinanzasApp",
            descripcion: "Confirma tu identidad para acceder");

        //Paso 2: Validacion si el usuario cancela la autenticacion, no se le bloquea el acceso a la app
        if (resultado.Cancelado)
        {
            return;
        }

        //Paso 3: Validacion si la autenticación fue exitosa
        if (resultado.Exitoso)
        {
            // Navega al shell principal reemplazando la pila de navegación
            if (Shell.Current != null)
            {
                Application.Current.MainPage = new AppShell();
            }
            else
            {
                Application.Current.MainPage = new AppShell();
            }
        }
        else
        {
            _intentosFallidos++;

            if (_intentosFallidos >= MaxIntentos)
            {
                await DisplayAlertAsync(
                    "Acceso bloqueado",
                    "Demasiados intentos fallidos. Por seguridad, la biometría ha sido desactivada temporalmente.",
                    "Continuar sin biometría");

                await Shell.Current.GoToAsync(nameof(AppShell));
            }
        }
    }

    private async void OnReintentar_Clicked(object sender, EventArgs e) =>
        await SolicitarAutenticacionAsync();
}
