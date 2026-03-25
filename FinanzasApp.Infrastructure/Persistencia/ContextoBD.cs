using FinanzasApp.Domain.Entidades;
using SQLite;

namespace FinanzasApp.Infraestructura.Persistencia;

/// <summary>
/// Contexto de base de datos SQLite.
/// Gestiona la conexión y asegura que las tablas existan al iniciar.
/// Usa el patrón Singleton para tener una sola conexión compartida.
/// </summary>
public class ContextoBD
{
    private SQLiteAsyncConnection? _conexion;

    /// <summary>
    /// Ruta física del archivo SQLite en el dispositivo.
    /// En Android: /data/data/com.empresa.finanzasapp/files/finanzas.db
    /// En iOS: dentro del sandbox de la app
    /// </summary>
    private static string RutaBaseDatos =>
        Path.Combine(FileSystem.AppDataDirectory, "finanzas.db");

    /// <summary>
    /// Inicializa la conexión y crea las tablas si no existen.
    /// Debe llamarse una vez al inicio de la aplicación.
    /// </summary>
    public async Task<SQLiteAsyncConnection> ObtenerConexionAsync()
    {
        if (_conexion is not null) return _conexion;

        // Configuración SQLite: modo WAL para mejor rendimiento concurrente
        var opciones = new SQLiteConnectionString(
            RutaBaseDatos,
            storeDateTimeAsTicks: true
        );

        _conexion = new SQLiteAsyncConnection(opciones);

        // Crea las tablas si no existen (idempotente)
        await _conexion.CreateTableAsync<Tarjeta>();
        await _conexion.CreateTableAsync<Transaccion>();

        return _conexion;
    }

    /// <summary>
    /// Cierra la conexión de forma limpia. Llamar al cerrar la app.
    /// </summary>
    public async Task CerrarAsync()
    {
        if (_conexion is not null)
        {
            await _conexion.CloseAsync();
            _conexion = null;
        }
    }
}
