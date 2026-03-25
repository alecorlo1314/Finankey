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
    }

    protected override async void OnStart()
    {
        base.OnStart();

        // Inicia la carga del modelo ONNX en segundo plano.
        // No bloqueamos el hilo principal; si tarda, la primera predicción
        // se hará cuando el usuario ya esté en el formulario.
        _ = Task.Run(async () => await _servicioPrediccion.InicializarAsync());

        // Determina la página inicial según la configuración biométrica
        MainPage = await DeterminarPaginaInicialAsync();
    }

    /// <summary>
    /// Decide la pantalla inicial:
    /// - Si biometría está habilitada y disponible → LoginBiometricoPage
    /// - En cualquier otro caso → AppShell (app completa)
    /// </summary>
    private async Task<Page> DeterminarPaginaInicialAsync()
    {
        var biometriaHabilitada = _servicioBiometrico.BiometriaHabilitada;
        var biometriaDisponible = await _servicioBiometrico.EsDisponibleAsync();

        if (biometriaHabilitada && biometriaDisponible)
        {
            // Resuelve la página desde el contenedor para inyectar dependencias
            return _servicios.GetRequiredService<LoginBiometricoPage>();
        }

        return new AppShell();
    }

    protected override Window CreateWindow(IActivationState? activationState) =>
        new(new ContentPage()) { Title = "FinanzasApp" };
}
