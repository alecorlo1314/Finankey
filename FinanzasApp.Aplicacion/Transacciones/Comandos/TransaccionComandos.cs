using FinanzasApp.Aplicacion.DTOs;
using FinanzasApp.Aplicacion.Interfaces;
using FinanzasApp.Domain.Entidades;
using FinanzasApp.Domain.Enumeraciones;
using FinanzasApp.Domain.Interfaces;

namespace FinanzasApp.Aplicacion.Transacciones.Comandos;

// ── Comandos ─────────────────────────────────────────────────────────────────

public record CrearTransaccionComando(TransaccionFormDto Datos) : IComando<int>;
public record ActualizarTransaccionComando(TransaccionFormDto Datos) : IComando<bool>;
public record EliminarTransaccionComando(int TransaccionId) : IComando<bool>;

// ── Manejadores de comandos ───────────────────────────────────────────────────

public class CrearTransaccionManejador(
    IRepositorioTransaccion repositorioTransaccion,
    IRepositorioTarjeta repositorioTarjeta)
    : IManejadorComando<CrearTransaccionComando, int>
{
    public async Task<int> ManejarAsync(CrearTransaccionComando comando, CancellationToken cancellationToken = default)
    {
        var datos = comando.Datos;

        var tarjeta = await repositorioTarjeta.ObtenerPorIdAsync(datos.TarjetaId)
            ?? throw new KeyNotFoundException($"Tarjeta {datos.TarjetaId} no encontrada.");

        var transaccion = new Transaccion
        {
            TarjetaId = datos.TarjetaId,
            Descripcion = datos.Descripcion.Trim(),
            Monto = Math.Abs(datos.Monto),          // Siempre positivo
            Tipo = datos.Tipo,
            Categoria = datos.Categoria,
            CategoriaPredicha = datos.CategoriaPredicha,
            ConfianzaPrediccion = datos.ConfianzaPrediccion,
            Fecha = datos.Fecha,
            Notas = datos.Notas?.Trim()
        };

        var id = await repositorioTransaccion.AgregarAsync(transaccion);

        // Actualiza el saldo de la tarjeta según el tipo de transacción
        var nuevoSaldo = datos.Tipo == TipoTransaccion.Gasto
            ? tarjeta.SaldoActual + datos.Monto      // Gasto: aumenta deuda/reduce saldo
            : tarjeta.SaldoActual - datos.Monto;     // Ingreso: reduce deuda/aumenta saldo

        await repositorioTarjeta.ActualizarSaldoAsync(datos.TarjetaId, nuevoSaldo);

        return id;
    }
}

public class ActualizarTransaccionManejador(
    IRepositorioTransaccion repositorioTransaccion,
    IRepositorioTarjeta repositorioTarjeta)
    : IManejadorComando<ActualizarTransaccionComando, bool>
{
    public async Task<bool> ManejarAsync(ActualizarTransaccionComando comando, CancellationToken cancellationToken = default)
    {
        if (!comando.Datos.Id.HasValue)
            throw new ArgumentException("Id requerido para actualizar.");

        var transaccionExistente = await repositorioTransaccion.ObtenerPorIdAsync(comando.Datos.Id.Value)
            ?? throw new KeyNotFoundException("Transacción no encontrada.");

        var tarjeta = await repositorioTarjeta.ObtenerPorIdAsync(transaccionExistente.TarjetaId)
            ?? throw new KeyNotFoundException("Tarjeta no encontrada.");

        // Revierte el efecto de la transacción anterior en el saldo
        var saldoRevertido = transaccionExistente.Tipo == TipoTransaccion.Gasto
            ? tarjeta.SaldoActual - transaccionExistente.Monto
            : tarjeta.SaldoActual + transaccionExistente.Monto;

        // Aplica el efecto de la nueva transacción
        var nuevoSaldo = comando.Datos.Tipo == TipoTransaccion.Gasto
            ? saldoRevertido + comando.Datos.Monto
            : saldoRevertido - comando.Datos.Monto;

        transaccionExistente.Descripcion = comando.Datos.Descripcion.Trim();
        transaccionExistente.Monto = Math.Abs(comando.Datos.Monto);
        transaccionExistente.Tipo = comando.Datos.Tipo;
        transaccionExistente.Categoria = comando.Datos.Categoria;
        transaccionExistente.CategoriaPredicha = comando.Datos.CategoriaPredicha;
        transaccionExistente.ConfianzaPrediccion = comando.Datos.ConfianzaPrediccion;
        transaccionExistente.Fecha = comando.Datos.Fecha;
        transaccionExistente.Notas = comando.Datos.Notas?.Trim();

        await repositorioTransaccion.ActualizarAsync(transaccionExistente);
        await repositorioTarjeta.ActualizarSaldoAsync(tarjeta.Id, nuevoSaldo);

        return true;
    }
}

public class EliminarTransaccionManejador(
    IRepositorioTransaccion repositorioTransaccion,
    IRepositorioTarjeta repositorioTarjeta)
    : IManejadorComando<EliminarTransaccionComando, bool>
{
    public async Task<bool> ManejarAsync(EliminarTransaccionComando comando, CancellationToken cancellationToken = default)
    {
        var transaccion = await repositorioTransaccion.ObtenerPorIdAsync(comando.TransaccionId)
            ?? throw new KeyNotFoundException("Transacción no encontrada.");

        var tarjeta = await repositorioTarjeta.ObtenerPorIdAsync(transaccion.TarjetaId)
            ?? throw new KeyNotFoundException("Tarjeta no encontrada.");

        // Revierte el saldo al eliminar la transacción
        var nuevoSaldo = transaccion.Tipo == TipoTransaccion.Gasto
            ? tarjeta.SaldoActual - transaccion.Monto
            : tarjeta.SaldoActual + transaccion.Monto;

        await repositorioTransaccion.EliminarAsync(comando.TransaccionId);
        await repositorioTarjeta.ActualizarSaldoAsync(tarjeta.Id, nuevoSaldo);

        return true;
    }
}
