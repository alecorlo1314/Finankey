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

        // Esperar un poco antes de verificar la biometría
        await Task.Delay(300);

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
        if (resultado.Cancelado) return;

        //Paso 3: Validacion si la autenticación fue exitosa
        if (resultado.Exitoso)
        {
            await CerrarLoginYEntrarAsync();
            return;
        }

        //Autenticación fallida, incrementar el contador de intentos
        _intentosFallidos++;

        if (_intentosFallidos >= MaxIntentos)
        {
            await DisplayAlertAsync(
                "Acceso bloqueado",
                "Demasiados intentos fallidos. Por seguridad, la biometría ha sido desactivada temporalmente.",
                "Continuar sin biometría");

            // Entra a la app sin biometría
            await CerrarLoginYEntrarAsync();
        }
    }

    /// <summary>
    /// Cierra el modal de login correctamente.
    /// Como el AppShell ya está debajo como MainPage,
    /// solo necesitamos cerrar el modal para revelar la app.
    /// </summary>
    private async Task CerrarLoginYEntrarAsync()
    {
        await MainThread.InvokeOnMainThreadAsync(async () =>
        {
            // PopModalAsync cierra el modal y revela el AppShell que está debajo
            // animated: false evita un segundo flash durante la transición
            await Navigation.PopModalAsync(animated: false);
        });
    }

    private async void OnReintentar_Clicked(object sender, EventArgs e) =>
        await SolicitarAutenticacionAsync();

    private async void BtnInicioSinBiometria_Clicked(object sender, EventArgs e)
    {
        await CerrarLoginYEntrarAsync();
    }
}
