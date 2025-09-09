using SQLite;

namespace FinanKey.Dominio.Interfaces
{
    public interface IServicioBaseDatos
    {
        Task<SQLiteAsyncConnection> ObtenerConexion();

        Task CerrarConexionAsync();

        Task EliminarBaseDatosAsync();

        Task ReconstruirBaseDatosAsync();
    }
}