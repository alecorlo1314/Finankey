using FinanzasApp.Aplicacion.DTOs;
using FinanzasApp.Aplicacion.Interfaces;
using FinanzasApp.Domain.Entidades;
using FinanzasApp.Domain.Interfaces;

namespace FinanzasApp.Aplicacion.Tarjetas.Comandos;

// ── Comandos ─────────────────────────────────────────────────────────────────

/// <summary>Comando para crear una nueva tarjeta</summary>
public record CrearTarjetaComando(TarjetaFormDto Datos) : IComando<int>;

/// <summary>Comando para actualizar los datos de una tarjeta existente</summary>
public record ActualizarTarjetaComando(TarjetaFormDto Datos) : IComando<bool>;

/// <summary>Comando para eliminar lógicamente una tarjeta (la desactiva)</summary>
public record EliminarTarjetaComando(int TarjetaId) : IComando<bool>;

// ── Manejadores ───────────────────────────────────────────────────────────────

/// <summary>
/// Crea una nueva tarjeta validando los datos y persistiéndola.
/// Retorna el Id asignado por la base de datos.
/// </summary>
public class CrearTarjetaManejador(IRepositorioTarjeta repositorio)
    : IManejadorComando<CrearTarjetaComando, int>
{
    public async Task<int> ManejarAsync(CrearTarjetaComando comando, CancellationToken cancellationToken = default)
    {
        var datos = comando.Datos;

        // Validación mínima de negocio
        if (string.IsNullOrWhiteSpace(datos.Nombre))
            throw new ArgumentException("El nombre de la tarjeta es requerido.");

        if (datos.UltimosDigitos.Length != 4 || !datos.UltimosDigitos.All(char.IsDigit))
            throw new ArgumentException("Los últimos dígitos deben ser exactamente 4 números.");

        var tarjeta = new Tarjeta
        {
            Nombre = datos.Nombre.Trim(),
            UltimosDigitos = datos.UltimosDigitos,
            Tipo = datos.Tipo,
            ColorHex = datos.ColorHex,
            Banco = datos.Banco.Trim(),
            RedTarjeta = datos.RedTarjeta,
            LimiteCredito = datos.LimiteCredito,
            SaldoActual = datos.SaldoActual,
            FechaRegistro = DateTime.Now,
            EstaActiva = true
        };

        return await repositorio.AgregarAsync(tarjeta);
    }
}

/// <summary>
/// Actualiza una tarjeta existente. Valida que exista antes de modificar.
/// </summary>
public class ActualizarTarjetaManejador(IRepositorioTarjeta repositorio)
    : IManejadorComando<ActualizarTarjetaComando, bool>
{
    public async Task<bool> ManejarAsync(ActualizarTarjetaComando comando, CancellationToken cancellationToken = default)
    {
        var datos = comando.Datos;

        if (!datos.Id.HasValue)
            throw new ArgumentException("El Id de la tarjeta es requerido para actualizar.");

        var tarjetaExistente = await repositorio.ObtenerPorIdAsync(datos.Id.Value)
            ?? throw new KeyNotFoundException($"No se encontró la tarjeta con Id {datos.Id.Value}.");

        // Solo actualizamos los campos que el usuario puede modificar
        tarjetaExistente.Nombre = datos.Nombre.Trim();
        tarjetaExistente.UltimosDigitos = datos.UltimosDigitos;
        tarjetaExistente.Tipo = datos.Tipo;
        tarjetaExistente.ColorHex = datos.ColorHex;
        tarjetaExistente.Banco = datos.Banco.Trim();
        tarjetaExistente.RedTarjeta = datos.RedTarjeta;
        tarjetaExistente.LimiteCredito = datos.LimiteCredito;
        tarjetaExistente.SaldoActual = datos.SaldoActual;

        await repositorio.ActualizarAsync(tarjetaExistente);
        return true;
    }
}

/// <summary>
/// Elimina lógicamente una tarjeta (la marca como inactiva).
/// No elimina físicamente para conservar el historial de transacciones.
/// </summary>
public class EliminarTarjetaManejador(IRepositorioTarjeta repositorio)
    : IManejadorComando<EliminarTarjetaComando, bool>
{
    public async Task<bool> ManejarAsync(EliminarTarjetaComando comando, CancellationToken cancellationToken = default)
    {
        var tarjeta = await repositorio.ObtenerPorIdAsync(comando.TarjetaId)
            ?? throw new KeyNotFoundException($"No se encontró la tarjeta con Id {comando.TarjetaId}.");

        tarjeta.EstaActiva = false;
        await repositorio.ActualizarAsync(tarjeta);
        return true;
    }
}
