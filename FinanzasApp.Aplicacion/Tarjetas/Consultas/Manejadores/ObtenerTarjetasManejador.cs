using FinanzasApp.Aplicacion.DTOs;
using FinanzasApp.Aplicacion.Interfaces;
using FinanzasApp.Domain.Entidades;
using FinanzasApp.Domain.Interfaces;

namespace FinanzasApp.Aplicacion.Tarjetas.Consultas.Manejadores;

public class ObtenerTarjetasManejador(IRepositorioTarjeta repositorio)
    : IManejadorConsulta<ObtenerTarjetasConsulta, IEnumerable<TarjetaResumenDto>?>
{
    public async Task<IEnumerable<TarjetaResumenDto>?> ManejarAsync(
        ObtenerTarjetasConsulta consulta,
        CancellationToken cancellationToken = default)
    {
        //Paso 1: Obtener todas las tarjetas activas
        //Esto nos devuelve una IEnumerable<Tarjeta> o null si no existen
        var tarjetas = await repositorio.ObtenerActivasAsync();

        //Paso 2: Si no existen tarjetas, retornamos null
        if (tarjetas is null) return null;

        //Paso 3: Si existen tarjetas, retornamos el IEnumerable<TarjetaResumenDto>
        return tarjetas.Select(MapearATarjetaResumenDto);
    }

    /// <summary>
    /// Convierte la entidad de dominio al DTO para la capa de presentación
    /// </summary>
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
        EstaActiva: t.EstaActiva,
        MesVencimiento: t.MesVencimiento,
        AnioVencimiento: t.AnioVencimiento,
        FechaVencimiento: t.FechaVencimiento,
        EstaVencida: t.EstaVencida,
        DiaCorte: t.DiaCorte,
        DiaPago: t.DiaPago,
        ProximoCorte: t.ProximoCorte,
        ProximoPago: t.ProximoPago,
        DiasParaCorte: t.DiasParaCorte,
        DiasParaPago: t.DiasParaPago
    );
}
