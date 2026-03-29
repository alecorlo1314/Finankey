using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Maui.Alerts;

namespace FinanzasApp.Presentacion.ViewModels;

/// <summary>
/// ViewModel base del que heredan todos los demás.
/// Centraliza: estado de carga, errores, navegación y helpers.
/// </summary>
public abstract partial class ViewModelBase : ObservableObject
{
    #region 🧾 Estado global de la UI

    /// Indica si hay una operación en progreso (spinner)
    [ObservableProperty]
    private bool _estaCargando;

    /// Mensaje de error visible en UI
    [ObservableProperty]
    private string? _mensajeError;

    /// Indica si existe un error activo
    [ObservableProperty]
    private bool _tieneError;

    /// Título de la página (usado por Shell)
    [ObservableProperty]
    private string _titulo = string.Empty;

    #endregion

    #region 🔄 Ciclo de vida

    /// Se ejecuta cuando la vista aparece
    public virtual Task AlAparecerAsync() => Task.CompletedTask;

    /// Se ejecuta cuando la vista desaparece
    public virtual Task AlDesaparecerAsync() => Task.CompletedTask;

    #endregion

    #region ⚙️ Manejo de ejecución segura (carga + errores)

    /// Ejecuta lógica async con manejo automático de:
    /// - loading
    /// - errores
    protected async Task EjecutarConCargaAsync(
        Func<Task> accion,
        string mensajeErrorPersonalizado = "Ocurrió un error inesperado.")
    {
        try
        {
            // Paso 1: Activar el estado de carga
            EstaCargando = true;

            // Paso 2: Limpia errores previos
            MensajeError = null;
            TieneError = false;

            //Paso 3: Ejecutar lógica que estan Func<Task> accion
            await accion();
        }
        catch (KeyNotFoundException ex)
        {
            MostrarError($"Registro no encontrado: {ex.Message}");
        }
        catch (ArgumentException ex)
        {
            MostrarError(ex.Message);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[VM Error] {ex}");
            MostrarError(mensajeErrorPersonalizado);
        }
        finally
        {
            // Paso 4: Desactivar el estado de carga
            EstaCargando = false;
        }
    }

    #endregion

    #region ⚠️ Manejo de errores

    /// Muestra error en UI
    protected void MostrarError(string mensaje)
    {
        MensajeError = mensaje;
        TieneError = true;
    }

    /// Limpia estado de error
    protected void LimpiarError()
    {
        MensajeError = null;
        TieneError = false;
    }

    #endregion

    #region 🎯 Mensaje de exito o fracaso
    protected async Task MostrarToastAsync(
    string mensaje,
    ToastDuration duration = ToastDuration.Short,
    double fontSize = 14,
    CancellationToken? token = null)
    {
        var cancellationToken = token ?? new CancellationTokenSource().Token;

        await Toast.Make(mensaje, duration, fontSize)
                   .Show(cancellationToken);
    }

    #endregion

    #region 🧭 Navegación

    /// Navega hacia atrás
    [RelayCommand]
    protected static async Task RegresarAsync() =>
        await Shell.Current.GoToAsync("..");

    #endregion

    #region 💬 Diálogos

    /// Muestra alerta simple
    protected static Task MostrarAlertaAsync(string titulo, string mensaje) =>
        Shell.Current.DisplayAlertAsync(titulo, mensaje, "Aceptar");

    /// Muestra confirmación (Sí / No)
    protected static Task<bool> ConfirmarAsync(string titulo, string mensaje) =>
        Shell.Current.DisplayAlertAsync(titulo, mensaje, "Sí", "No");

    #endregion
}