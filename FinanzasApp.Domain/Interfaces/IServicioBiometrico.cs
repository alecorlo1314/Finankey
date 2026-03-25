namespace FinanzasApp.Domain.Interfaces;

/// <summary>
/// Contrato para el servicio de autenticación biométrica.
/// La implementación varía según el sistema operativo (Android/iOS).
/// </summary>
public interface IServicioBiometrico
{
    /// <summary>
    /// Verifica si el dispositivo soporta autenticación biométrica
    /// (huella digital, Face ID, etc.)
    /// </summary>
    Task<bool> EsDisponibleAsync();

    /// <summary>
    /// Solicita autenticación biométrica al usuario.
    /// </summary>
    /// <param name="titulo">Título del diálogo de autenticación</param>
    /// <param name="descripcion">Mensaje explicativo para el usuario</param>
    /// <returns>true si la autenticación fue exitosa</returns>
    Task<ResultadoBiometrico> AutenticarAsync(string titulo, string descripcion);

    /// <summary>Indica si el usuario ha habilitado la biometría en la app</summary>
    bool BiometriaHabilitada { get; set; }
}

/// <summary>Resultado de un intento de autenticación biométrica</summary>
public record ResultadoBiometrico(
    bool Exitoso,
    string? MensajeError = null,
    TipoBiometria TipoBiometria = TipoBiometria.Ninguna
);

/// <summary>Tipo de biometría disponible en el dispositivo</summary>
public enum TipoBiometria
{
    Ninguna,
    HuellaDigital,
    FaceId,
    IrisEscaner,
    Combinada
}
