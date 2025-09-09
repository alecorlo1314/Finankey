
using FinanKey.Dominio.Interfaces;
using FinanKey.Dominio.Models;

namespace FinanKey.Infraestructura.Repositorios
{
    public class RespositorioMovimiento : IServicioMovimiento
    {
        private readonly RepositorioBaseDatos _repositorioBaseDatos;

        //Inyeccion de dependencias para el servicio de base de datos
        public RespositorioMovimiento(RepositorioBaseDatos repositorioBaseDatos)
        {
            _repositorioBaseDatos = repositorioBaseDatos;
        }

        // Business rule: registering an expense/income updates balances per card type.
        public async Task<int> AgregarMovimientoAsync(Movimiento NuevoMovimiento)
        {
            //Obtenemos la conexion a la base de datos
            var conexion = await _repositorioBaseDatos.ObtenerConexion();
            //Inicializamos el id del movimiento
            int resultadoFinal = 0;

            //Guardamos el movimiento en la base de datos
            await conexion.InsertAsync(NuevoMovimiento);

            //validamos si el movimiento es un gasto o ingreso y si tiene tarjeta asociada
            if (NuevoMovimiento.TipoMovimiento == "Gasto")
            {
                // buscamos la tarjeta asociada al movimiento para deducir el monto de esa tarjeta
                var tarjetaAsociada = await conexion.FindAsync<Tarjeta>(NuevoMovimiento.TarjetaId);
                // si la tarjeta existe, actualizamos su saldo
                if (tarjetaAsociada != null)
                {
                    // si es un gasto, actualizamos el saldo de la tarjeta
                    if (tarjetaAsociada.Tipo == "Debito")
                    {
                        //si es debito o corriente, restamos el monto del gasto al saldo actual
                        tarjetaAsociada.MontoInicial -= Math.Abs(NuevoMovimiento.Monto);
                        // si es un gasto con tarjeta de debito o corriente, actualizamos el estado de pago
                        await conexion.UpdateAsync(tarjetaAsociada);
                        // si es un gasto con tarjeta de credito, actualizamos el estado de pago
                        NuevoMovimiento.EsPagado = null; // no aplica para debito
                                                         // guardamos el movimiento
                        return resultadoFinal = await conexion.UpdateAsync(NuevoMovimiento);
                    }
                    else if (tarjetaAsociada.Tipo.Contains(Enums.TipoTarjeta.Credito.ToString()))
                    {
                        // si es credito, restamos el monto del gasto al saldo pendiente
                        //card.SaldoPendiente += Math.Abs(NuevoMovimiento.Monto);
                        // si es un gasto con tarjeta de credito, actualizamos el estado de pago
                        await conexion.UpdateAsync(tarjetaAsociada);
                        // si es un gasto con tarjeta de credito, actualizamos el estado de pago
                        NuevoMovimiento.EsPagado ??= false;
                        //Actualizamos el estado del movimiento
                        await conexion.UpdateAsync(NuevoMovimiento);
                    }
                }
            }
            // si es un ingreso y tiene tarjeta asociada, actualizamos el saldo de la tarjeta
            else if (NuevoMovimiento.TipoMovimiento.Contains(Enums.TipoMovimiento.Ingreso.ToString()))
            {
                // buscamos la tarjeta asociada al movimiento
                var card = await conexion.FindAsync<Tarjeta>(NuevoMovimiento.TarjetaId);
                // si la tarjeta existe, actualizamos su saldo
                if (card != null && (card.Tipo == "Debito" || card.Tipo == "Corriente"))
                {
                    // si es debito o corriente, sumamos el monto del ingreso al saldo actual
                    //card.SaldoActual += Math.Abs(NuevoMovimiento.Monto);
                    // guardamos el movimiento
                    await conexion.UpdateAsync(card);
                }
            }
            return resultadoFinal;
        }

        public async Task ActualizarMovimientoAsync(Movimiento MovimientoActualizado)
        {
            var conexion = await _repositorioBaseDatos.ObtenerConexion();
            await conexion.UpdateAsync(MovimientoActualizado);
        }

        public async Task EliminarMovimientoAsync(Movimiento EliminarMovimiento)
        {
            var conexion = await _repositorioBaseDatos.ObtenerConexion();
            await conexion.DeleteAsync(EliminarMovimiento);
        }

        public async Task<List<Movimiento>> ObtenerMovimientosAsync()
        {
            var conexion = await _repositorioBaseDatos.ObtenerConexion();
            return await conexion.Table<Movimiento>().OrderByDescending(m => m.FechaMovimiento).ToListAsync();
        }

        public async Task<Movimiento?> ObtenerMovimientoPorIdAsync(int IdMovimiento)
        {
            var conexion = await _repositorioBaseDatos.ObtenerConexion();
            return await conexion.FindAsync<Movimiento>(IdMovimiento);
        }
        public async Task<List<Movimiento>> ListaGastosPorTarjetaAsync(int TarjetaId)
        {
            var conexion = await _repositorioBaseDatos.ObtenerConexion();
            return await conexion.Table<Movimiento>().Where(m => m.TarjetaId == TarjetaId).OrderByDescending(m => m.FechaMovimiento).ToListAsync();
        }
    }
}