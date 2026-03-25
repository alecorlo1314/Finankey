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

        // Pequeña pausa para que la UI se renderice antes de mostrar el prompt
        await Task.Delay(300);
        await SolicitarAutenticacionAsync();
    }

    /// <summary>
    /// Solicita autenticación biométrica. Si falla repetidamente,
    /// desbloquea la opción de entrar sin biometría.
    /// </summary>
    private async Task SolicitarAutenticacionAsync()
    {
        var resultado = await _servicioBiometrico.AutenticarAsync(
            titulo: "FinanzasApp",
            descripcion: "Confirma tu identidad para acceder");

        if (resultado.Exitoso)
        {
            // Navega al shell principal reemplazando la pila de navegación
            await Shell.Current.GoToAsync(nameof(AppShell));
        }
        else
        {
            _intentosFallidos++;

            if (_intentosFallidos >= MaxIntentos)
            {
                // Después de 3 fallos, permite entrar sin biometría
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
