using FinanKey.Dominio.Models;
using FinanKey.Dominio.Interfaces;

namespace FinanKey.Infraestructura.Repositorios
{
    public class ServicioTarjeta : IServicioTarjeta
    {
        private readonly ServicioBaseDatos _servicioBaseDatos;
        //Inyeccion de dependencias para el servicio de base de datos
        public ServicioTarjeta(ServicioBaseDatos servicioBaseDatos) => _servicioBaseDatos = servicioBaseDatos;

        #region Metodo Agregar Tarjeta
        public async Task<int> AgregarAsync(Tarjeta Nuevatarjeta)
        {
            //Obtenemos la conexion a la base de datos
            var conexion = await _servicioBaseDatos.ObtenerConexion();
            //Retornamos un 1 si se ingreso y un 0 si no se pudo ingresar
            return await conexion.InsertAsync(Nuevatarjeta);
        }
        #endregion

        #region Metodo Actualizar Tarjeta
        public async Task ActualizarAsync(Tarjeta TarjetaActualizada)
        {
            //Obtenemos la conexion a la base de datos
            var conexion = await _servicioBaseDatos.ObtenerConexion();
            //Validamos que el alias no sea nulo o vacio
            await conexion.UpdateAsync(TarjetaActualizada);
        }
        #endregion

        #region Metodo para eliminar una tarjeta por su id
        public async Task EliminarAsync(int idTarjeta)
        {
            //Obtenemos la conexion a la base de datos
            var conexion = await _servicioBaseDatos.ObtenerConexion();
            //Eliminamos la tarjeta por su id y retornamos el resultado si fue afectado al menos una fila
            await conexion.DeleteAsync<Tarjeta>(idTarjeta);
        }
        #endregion

        #region Metodo para obtener todas las tarjetas
        public async Task<List<Tarjeta>> ObtenerTarjetasAsync()
        {
            //Obtenemos la conexion a la base de datos
            var conexion = await _servicioBaseDatos.ObtenerConexion();
            //Retornamos todas las tarjetas ordenadas por si es predeterminada
            return await conexion.Table<Tarjeta>().OrderByDescending(c => c.FechaCreacion).ToListAsync();
        }
        #endregion

        #region Metodo para obtener una tarjeta por su id
        public async Task<Tarjeta?> ObtenerTarjetaPorIdAsync(int idTarjeta)
        {
            //Obtenemos la conexion a la base de datos
            var conexion = await _servicioBaseDatos.ObtenerConexion();
            //Retornamos la tarjeta por su id
            return await conexion.FindAsync<Tarjeta>(idTarjeta);
        }
        #endregion
    }
}

