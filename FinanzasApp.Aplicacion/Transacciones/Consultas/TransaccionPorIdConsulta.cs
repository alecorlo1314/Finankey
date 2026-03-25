using FinanzasApp.Aplicacion.DTOs;
using FinanzasApp.Aplicacion.Interfaces;
using FinanzasApp.Domain.Entidades;
using FinanzasApp.Domain.Interfaces;

namespace FinanzasApp.Aplicacion.Transacciones.Consultas;

// ── Consulta faltante para edición ────────────────────────────────────────────

/// <summary>
/// Obtiene una transacción por su Id para cargarla en el formulario de edición.
/// </summary>
public record ObtenerTransaccionPorIdConsulta(int TransaccionId)
    : IConsulta<TransaccionResumenDto?>;

public class ObtenerTransaccionPorIdManejador(
    IRepositorioTransaccion repositorioTransaccion,
    IRepositorioTarjeta repositorioTarjeta)
    : IManejadorConsulta<ObtenerTransaccionPorIdConsulta, TransaccionResumenDto?>
{
    public async Task<TransaccionResumenDto?> ManejarAsync(
        ObtenerTransaccionPorIdConsulta consulta,
        CancellationToken cancellationToken = default)
    {
        var transaccion = await repositorioTransaccion.ObtenerPorIdAsync(consulta.TransaccionId);
        if (transaccion is null) return null;

        var tarjeta = await repositorioTarjeta.ObtenerPorIdAsync(transaccion.TarjetaId);

        return new TransaccionResumenDto(
            Id: transaccion.Id,
            TarjetaId: transaccion.TarjetaId,
            NombreTarjeta: tarjeta?.Nombre ?? "Tarjeta eliminada",
            Descripcion: transaccion.Descripcion,
            Monto: transaccion.Monto,
            Tipo: transaccion.Tipo,
            Categoria: transaccion.Categoria,
            CategoriaPredicha: transaccion.CategoriaPredicha,
            ConfianzaPrediccion: transaccion.ConfianzaPrediccion,
            Fecha: transaccion.Fecha,
            Notas: transaccion.Notas
        );
    }
}
