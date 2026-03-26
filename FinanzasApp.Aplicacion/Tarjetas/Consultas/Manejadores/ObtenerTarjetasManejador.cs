using FinanzasApp.Aplicacion.DTOs;
using FinanzasApp.Aplicacion.Interfaces;
using FinanzasApp.Domain.Entidades;
using FinanzasApp.Domain.Interfaces;

namespace FinanzasApp.Aplicacion.Tarjetas.Consultas.Manejadores;

public class ObtenerTarjetasManejador(IRepositorioTarjeta repositorio)
    : IManejadorConsulta<ObtenerTarjetasConsulta, IEnumerable<TarjetaResumenDto>>
{
    public async Task<IEnumerable<TarjetaResumenDto>> ManejarAsync(
        ObtenerTarjetasConsulta consulta,
        CancellationToken cancellationToken = default)
    {
        var tarjetas = await repositorio.ObtenerActivasAsync();
        return tarjetas.Select(MapearATarjetaResumenDto);
    }

    /// <summary>Convierte la entidad de dominio al DTO para la capa de presentación</summary>
    private static TarjetaResumenDto MapearATarjetaResumenDto(Tarjeta t) => new(
        Id: t.Id,
        Nombre: t.Nombre,
        UltimosDigitos: t.UltimosDigitos,
        Tipo: t.Tipo,
        ColorHex: t.ColorHex,
        Banco: t.Banco,
        RedTarjeta: t.RedTarjeta,
        SaldoActual: t.SaldoActual,
        LimiteCredito: t.LimiteCredito,
        CreditoDisponible: t.CreditoDisponible,
        PorcentajeUso: t.PorcentajeUso,
        EstaActiva: t.EstaActiva
    );
}
