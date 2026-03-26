using FinanzasApp.Aplicacion.Interfaces;
using FinanzasApp.Domain.Interfaces;

namespace FinanzasApp.Aplicacion.Tarjetas.Comandos.Manejadores;

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
