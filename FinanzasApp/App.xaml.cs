using FinanzasApp.Domain.Interfaces;
using FinanzasApp.Presentacion.Vistas;

namespace FinanzasApp;

/// <summary>
/// Punto de entrada principal de la aplicación MAUI.
/// Decide si mostrar la pantalla de login biométrico o ir directo al Shell.
/// También inicializa el modelo ONNX de forma asíncrona al arrancar.
/// </summary>
public partial class App : Application
{
    private readonly IServicioBiometrico _servicioBiometrico;
    private readonly IServicioPrediccion _servicioPrediccion;
    private readonly IServiceProvider _servicios;

    public App(
        IServicioBiometrico servicioBiometrico,
        IServicioPrediccion servicioPrediccion,
        IServiceProvider servicios)
    {
        InitializeComponent();

        _servicioBiometrico = servicioBiometrico;
        _servicioPrediccion = servicioPrediccion;
        _servicios = servicios;

        //licencia sycnfusion
        Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("Ngo9BigBOggjHTQxAR8/V1JHaF5cWWdCf1FpRmJGdld5fUVHYVZUTXxaS00DNHVRdkdlWXxccnRUQmJeUkZyVkpWYEo=");

        MainPage = new AppShell();
    }

    protected override async void OnStart()
    {
        base.OnStart();

        // Inicia la carga del modelo ONNX en segundo plano.
        // No bloqueamos el hilo principal; si tarda, la primera predicción
        // se hará cuando el usuario ya esté en el formulario.
        _ = Task.Run(async () => await _servicioPrediccion.InicializarAsync());

        // Mostrar la pantalla de login biometrico si aplica
        _ = MostrarLoginBiometricoSiAplicaAsync();
    }

    /// <summary>
    /// Si la biometría está habilitada y disponible, muestra
    /// LoginBiometricoPage como modal sobre el AppShell.
    /// </summary>
    private async Task MostrarLoginBiometricoSiAplicaAsync()
    {
        //Paso 1: Verificar si la biometria esta habilitada en nuestro celular
        //Retorna un boleano
        var biometriaHabilitada = _servicioBiometrico.BiometriaHabilitada;

        //Paso 2: Verificar si la biometria esta disponible en nuestro celular
        //Retorna un boleano
        var biometriaDisponible = await _servicioBiometrico.EsDisponibleAsync();

        //Paso 3: solamente cuando este la biometria habilitada y disponible
        if (!biometriaHabilitada || !biometriaDisponible) return;

        //Paso 4: Mostrar la pantalla de login biometrico
        var paginaBiometrica = _servicios.GetService(typeof(LoginBiometricoPage)) as LoginBiometricoPage;

        //Paso 5: Si la pagina biometrica no es null, mostrarla como modal
        if (paginaBiometrica is null) return;

        // Muestra el login como modal sobre el Shell ya renderizado
        // El usuario no puede cerrar este modal con el gesto de swipe
        await MainThread.InvokeOnMainThreadAsync(async () =>
        {
            await MainPage!.Navigation.PushModalAsync(paginaBiometrica, animated: false);
        });
    }
}
