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
            //Paso 1: Verificar la actual biometría habilitada
            var biometria = BiometricAuthentication.Current;

            //Paso 2: Verificar si el dispositivo soporta biometría
            var habilitada = await biometria.IsAvailableAsync();

            //Paso 3: Retornar el resultado booleano
            return habilitada;
        }
        catch
        {
            //Si algo falla, retornar falso
            return false;
        }
    }

    public async Task<ResultadoBiometrico> AutenticarAsync(string titulo, string descripcion)
    {
        try
        {
            //Paso 1: Verificar la actual biometría habilitada
            var biometria = BiometricAuthentication.Current;

            //Paso 2: Verificar si el dispositivo soporta biometría
            var disponibility = await biometria.IsAvailableAsync();

            //Paso 3: Si no es disponible, mostrar alerta y retornar resultado de error
            if (!disponibility)
            {
                return new ResultadoBiometrico(
                    Exitoso: false,
                    MensajeError: "Biometría no disponible en este dispositivo");
            }

            //Paso 4: Intentar autenticar al usuario con biometría
            var resultado = await biometria.AuthenticateAsync(
                    new AuthenticationRequest(
                        title: "Autenticación requerida",
                        reason: "Por favor autentícate para continuar"

                    ));

            //Paso 5: Si el usuario cancela, retornar resultado indicando cancelación
            if (resultado.Status == AuthenticationStatus.Canceled)
            {
                return new ResultadoBiometrico(
                    Exitoso: false,
                    Cancelado: true);
            }

            //Paso 6: Si la autenticación fue exitosa, retornar el resultado
            if (resultado.IsSuccessful)
            {
                var tipo = await DeterminarTipoBiometriaAsync();
                return new ResultadoBiometrico(Exitoso: true, TipoBiometria: tipo);
            }

            //Paso 7: Si la autenticación falla, retornar el resultado de error
            return new ResultadoBiometrico(
                Exitoso: false,
                MensajeError: resultado.ErrorMessage ?? "Autenticación fallida"
            );
        }
        catch (Exception ex)
        {
            //Si algo falla, retornar el resultado de error
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
            //Paso 1: Esperar a que se complete la tarea
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
