using FinanKey.Models;
using FinanKey.Servicios;

namespace FinanKey.Datos
{
    public class ServicioTarjeta : IServicioTarjeta
    {
        private readonly ServicioBaseDatos _servicioBaseDatos;
        //Inyeccion de dependencias para el servicio de base de datos
        public ServicioTarjeta(ServicioBaseDatos servicioBaseDatos) => _servicioBaseDatos = servicioBaseDatos;
        //Metodo para agregar una nueva tarjeta a la base de datos
        public async Task<int> AgregarAsync(Tarjeta Nuevatarjeta)
        {
            //Obtenemos la conexion a la base de datos
            var conexion = await _servicioBaseDatos.ObtenerConexion();
            //Validamos que el alias no sea nulo o vacio
            Nuevatarjeta.Alias = Nuevatarjeta.Alias.Trim();
            //Validamos que el alias tenga al menos 3 caracteres
            Nuevatarjeta.UltimosCuatroDigitos = (Nuevatarjeta.UltimosCuatroDigitos ?? "").Trim().PadLeft(4).Substring(Math.Max(0, (Nuevatarjeta.UltimosCuatroDigitos ?? "").Length - 4));
            // Validamos que el tipo de tarjeta sea uno de los permitidos
            Nuevatarjeta.Tipo = Nuevatarjeta.Tipo is "Credito" or "Debito" or "Corriente" ? Nuevatarjeta.Tipo : "Credito";
            // Validamos que el color sea un color hexadecimal válido
            if (Nuevatarjeta.Tipo != "Credito") Nuevatarjeta.SaldoPendiente = 0m;
            // Validamos que el saldo actual sea mayor o igual a 0
            if (Nuevatarjeta.Tipo == "Credito" && Nuevatarjeta.SaldoActual != 0) Nuevatarjeta.SaldoActual = 0m;

            //retornamos el id de la tarjeta insertada
            return await conexion.InsertAsync(Nuevatarjeta);
        }
        //Metodo para actualizar una tarjeta existente en la base de datos
        public async Task ActualizarAsync(Tarjeta TarjetaActualizada)
        {
            //Obtenemos la conexion a la base de datos
            var conexion = await _servicioBaseDatos.ObtenerConexion();
            //Validamos que el alias no sea nulo o vacio
            await conexion.UpdateAsync(TarjetaActualizada);
        }

        //Metodo para eliminar una tarjeta de la base de datos
        public async Task EliminarAsync(int idTarjeta)
        {
            //Obtenemos la conexion a la base de datos
            var conexion = await _servicioBaseDatos.ObtenerConexion();
            //Eliminamos la tarjeta por su id y retornamos el resultado si fue afectado al menos una fila
            await conexion.DeleteAsync<Tarjeta>(idTarjeta);
        }
        //Metodo para obtener todas las tarjetas de la base de datos
        public async Task<List<Tarjeta>> ObtenerTarjetasAsync()
        {
            //Obtenemos la conexion a la base de datos
            var conexion = await _servicioBaseDatos.ObtenerConexion();
            //Retornamos todas las tarjetas ordenadas por si es predeterminada
            return await conexion.Table<Tarjeta>().OrderByDescending(c => c.EsPredeterminada).ToListAsync();
        }
        //Metodo para obtener una tarjeta por su id
        public async Task<Tarjeta?> ObtenerTarjetaPorIdAsync(int idTarjeta)
        {
            //Obtenemos la conexion a la base de datos
            var conexion = await _servicioBaseDatos.ObtenerConexion();
            //Retornamos la tarjeta por su id
            return await conexion.FindAsync<Tarjeta>(idTarjeta);
        }
    }
}

