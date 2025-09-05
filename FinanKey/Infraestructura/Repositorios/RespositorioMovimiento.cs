using FinanKey.Dominio.Interfaces;
using FinanKey.Dominio.Models;

namespace FinanKey.Infraestructura.Repositorios
{
    public class RespositorioMovimiento : IServicioMovimiento
    {
        private readonly ServicioBaseDatos _servicioBaseDatos;
        //Inyeccion de dependencias para el servicio de base de datos
        public RespositorioMovimiento(ServicioBaseDatos servicioBaseDatos) => _servicioBaseDatos = servicioBaseDatos;

        // Business rule: registering an expense/income updates balances per card type.
        public async Task<int> CrearMovimientoAsync(Movimiento NuevoMovimiento)
        {
            NuevoMovimiento.Fecha = NuevoMovimiento.Fecha.ToUniversalTime();
            //Obtenemos la conexion a la base de datos
            var conexion = await _servicioBaseDatos.ObtenerConexion();
            // Validamos que el monto sea mayor a 0
            int nuevoID = 0;
            await _servicioBaseDatos.CorrerEnTransicionAsync(async c =>
            {
                //Guardamos el movimiento en la base de datos
                nuevoID = await c.InsertAsync(NuevoMovimiento);
                //validamos si el movimiento es un gasto o ingreso y si tiene tarjeta asociada
                if (NuevoMovimiento.Tipo == "Gasto" && NuevoMovimiento.TarjetaId.HasValue)
                {
                    // buscamos la tarjeta asociada al movimiento
                    var card = await c.FindAsync<Tarjeta>(NuevoMovimiento.TarjetaId.Value);
                    // si la tarjeta existe, actualizamos su saldo
                    if (card != null)
                    {
                        // si es un gasto, actualizamos el saldo de la tarjeta
                        if (card.Tipo == "Debito" || card.Tipo == "Corriente")
                        {
                            //si es debito o corriente, restamos el monto del gasto al saldo actual
                            //card.SaldoActual -= Math.Abs(NuevoMovimiento.Monto);
                            // si es un gasto con tarjeta de credito, actualizamos el saldo pendiente
                            await c.UpdateAsync(card);
                            // si es un gasto con tarjeta de credito, actualizamos el estado de pago
                            NuevoMovimiento.EsPagado = null; // not applicable
                            // guardamos el movimiento
                            await c.UpdateAsync(NuevoMovimiento);
                        }
                        else if (card.Tipo == "Credito")
                        {
                            // si es credito, restamos el monto del gasto al saldo pendiente
                            //card.SaldoPendiente += Math.Abs(NuevoMovimiento.Monto);
                            // si es un gasto con tarjeta de credito, actualizamos el estado de pago
                            await c.UpdateAsync(card);
                            // si es un gasto con tarjeta de credito, actualizamos el estado de pago
                            NuevoMovimiento.EsPagado ??= false;
                            //Actualizamos el estado del movimiento
                            await c.UpdateAsync(NuevoMovimiento);
                        }
                    }
                }
                // si es un ingreso y tiene tarjeta asociada, actualizamos el saldo de la tarjeta
                else if (NuevoMovimiento.Tipo == "Ingreso" && NuevoMovimiento.TarjetaId.HasValue)
                {
                    // buscamos la tarjeta asociada al movimiento
                    var card = await c.FindAsync<Tarjeta>(NuevoMovimiento.TarjetaId.Value);
                    // si la tarjeta existe, actualizamos su saldo
                    if (card != null && (card.Tipo == "Debito" || card.Tipo == "Corriente"))
                    {
                        // si es debito o corriente, sumamos el monto del ingreso al saldo actual
                        //card.SaldoActual += Math.Abs(NuevoMovimiento.Monto);
                        // guardamos el movimiento
                        await c.UpdateAsync(card);
                    }
                }
            });

            return nuevoID;
        }

        public async Task ActualizarMovimientoAsync(Movimiento MovimientoActualizado)
        {
            var conexion = await _servicioBaseDatos.ObtenerConexion();
            await conexion.UpdateAsync(MovimientoActualizado);
        }

        public async Task EliminarMovimientoAsync(Movimiento EliminarMovimiento)
        {
            await _servicioBaseDatos.CorrerEnTransicionAsync(async c =>
            {
                // roll back balances when deleting if needed
                if (EliminarMovimiento.TarjetaId.HasValue)
                {
                    var card = await c.FindAsync<Tarjeta>(EliminarMovimiento.TarjetaId.Value);
                    if (card != null)
                    {
                        if (EliminarMovimiento.Tipo == "Gasto")
                        {
                            if (card.Tipo == "Debito" || card.Tipo == "Corriente") { }
                            //card.SaldoActual += Math.Abs(EliminarMovimiento.Monto);
                            else if (card.Tipo == "Credito")
                                //card.SaldoPendiente -= Math.Abs(EliminarMovimiento.Monto);
                                await c.UpdateAsync(card);
                        }
                        else if (EliminarMovimiento.Tipo == "Ingreso" && (card.Tipo == "Debito" || card.Tipo == "Corriente"))
                        {
                            //card.SaldoActual -= Math.Abs(EliminarMovimiento.Monto);
                            await c.UpdateAsync(card);
                        }
                    }
                }

                await c.DeleteAsync(EliminarMovimiento);
            });
        }

        public async Task<List<Movimiento>> ObtenerMovimientosAsync()
        {
            var conexion = await _servicioBaseDatos.ObtenerConexion();
            return await conexion.Table<Movimiento>().OrderByDescending(m => m.Fecha).ToListAsync();
        }

        public async Task<Movimiento?> ObtenerMovimientoPorIdAsync(int IdMovimiento)
        {
            var conexion = await _servicioBaseDatos.ObtenerConexion();
            return await conexion.FindAsync<Movimiento>(IdMovimiento);
        }

        public async Task<List<Movimiento>> ListaCreditosPendientesAsync()
        {
            var conexion = await _servicioBaseDatos.ObtenerConexion();
            return await conexion.Table<Movimiento>()
                .Where(m => m.Tipo == "Gasto" && m.EsPagado == false && m.TarjetaId.HasValue)
                .OrderByDescending(m => m.Fecha)
                .ToListAsync();
        }

        public async Task<List<Movimiento>> ListaGastosPorTarjetaAsync(int TarjetaId)
        {
            var conexion = await _servicioBaseDatos.ObtenerConexion();
            return await conexion.Table<Movimiento>().Where(m => m.TarjetaId == TarjetaId).OrderByDescending(m => m.Fecha).ToListAsync();
        }
    }
}
