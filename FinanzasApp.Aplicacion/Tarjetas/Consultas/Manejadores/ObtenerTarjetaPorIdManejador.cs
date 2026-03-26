using FinanzasApp.Aplicacion.DTOs;
using FinanzasApp.Aplicacion.Interfaces;
using FinanzasApp.Domain.Interfaces;

namespace FinanzasApp.Aplicacion.Tarjetas.Consultas.Manejadores;

public class ObtenerTarjetaPorIdManejador(IRepositorioTarjeta repositorio)
    : IManejadorConsulta<ObtenerTarjetaPorIdConsulta, TarjetaResumenDto?>
{
    public async Task<TarjetaResumenDto?> ManejarAsync(
        ObtenerTarjetaPorIdConsulta consulta,
        CancellationToken cancellationToken = default)
    {
        var tarjeta = await repositorio.ObtenerPorIdAsync(consulta.TarjetaId);
        if (tarjeta is null) return null;

        return new TarjetaResumenDto(
            Id: tarjeta.Id,
            Nombre: tarjeta.Nombre,
            UltimosDigitos: tarjeta.UltimosDigitos,
            Tipo: tarjeta.Tipo,
            ColorHex: tarjeta.ColorHex,
            Banco: tarjeta.Banco,
            RedTarjeta: tarjeta.RedTarjeta,
            SaldoActual: tarjeta.SaldoActual,
            LimiteCredito: tarjeta.LimiteCredito,
            CreditoDisponible: tarjeta.CreditoDisponible,
            PorcentajeUso: tarjeta.PorcentajeUso,
            EstaActiva: tarjeta.EstaActiva
        );
    }
}