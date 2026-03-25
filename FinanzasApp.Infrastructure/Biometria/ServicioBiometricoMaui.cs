using Maui.Biometric;

namespace FinanzasApp.Infraestructura.Biometria;

/// <summary>
/// Implementación del servicio biométrico usando el API de MAUI.
/// En iOS usa Face ID o Touch ID. En Android usa biometría del sistema.
/// Las preferencias de habilitación se persisten con Preferences.
/// </summary>
public class ServicioBiometricoMaui : IServicioBiometrico
{
    private const string ClavePreferenciaBiometria = "biometria_habilitada";

    /// <summary>Estado de habilitación persistido en las preferencias del dispositivo</summary>
    public bool BiometriaHabilitada
    {
        get => Preferences.Default.Get(ClavePreferenciaBiometria, false);
        set => Preferences.Default.Set(ClavePreferenciaBiometria, value);
    }

    public async Task<bool> EsDisponibleAsync()
    {
        try
        {
            var biometria = BiometricAuthentication.Current;

            var habilitada = await biometria.IsAvailableAsync();

            return habilitada;
            //// Verifica disponibilidad nativa de biometría en el dispositivo
            //return await MainThread.InvokeOnMainThreadAsync(async () =>
            //{
            //    var biometric = await BiometricAuthentication.Current.


            //    //var disponibilidad = await BiometricAuthentication.GetAvailabilityAsync();
            //    //return disponibilidad == BiometricAvailability.Available;
            //});
        }
        catch
        {
            return false;
        }
    }

    public async Task<ResultadoBiometrico> AutenticarAsync(string titulo, string descripcion)
    {
        try
        {
            var resultado = await BiometricAuthentication.Current.AuthenticateAsync(
                    new AuthenticationRequest(
                        title: "Autenticación requerida",
                        reason: "Por favor autentícate para continuar"
                        
                    ));
            //new AuthenticationRequest
            //{
            //    Title = titulo,
            //    NegativeText = "Cancelar",
            //    AllowPasswordFallback = true
            //});

            if (resultado.IsSuccessful)
            {
                var tipo = await DeterminarTipoBiometriaAsync();
                return new ResultadoBiometrico(Exitoso: true, TipoBiometria: tipo);
            }

            return new ResultadoBiometrico(
                Exitoso: false,
                MensajeError: resultado.ErrorMessage ?? "Autenticación fallida"
            );
        }
        catch (Exception ex)
        {
            return new ResultadoBiometrico(Exitoso: false, MensajeError: ex.Message);
        }
    }

    /// <summary>
    /// Determina el tipo de biometría disponible según el dispositivo.
    /// iOS reporta Face ID o Touch ID. Android reporta tipo genérico.
    /// </summary>
    private static async Task<TipoBiometria> DeterminarTipoBiometriaAsync()
    {
        try
        {
            await Task.CompletedTask;

#if IOS
            // En iOS se puede consultar el tipo de autenticador disponible
            var contexto = new LocalAuthentication.LAContext();
            return contexto.CanEvaluatePolicy(
                LocalAuthentication.LAPolicy.DeviceOwnerAuthenticationWithBiometrics,
                out _)
                ? contexto.BiometryType == LocalAuthentication.LABiometryType.FaceId
                    ? TipoBiometria.FaceId
                    : TipoBiometria.HuellaDigital
                : TipoBiometria.Ninguna;
#elif ANDROID
            return TipoBiometria.HuellaDigital;
#else
            return TipoBiometria.Ninguna;
#endif
        }
        catch
        {
            return TipoBiometria.Ninguna;
        }
    }
}
