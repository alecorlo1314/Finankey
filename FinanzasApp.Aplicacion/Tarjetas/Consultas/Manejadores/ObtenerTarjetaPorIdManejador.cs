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
        //Paso 1: Obtener la tarjeta por su ID
        //Esto nos devuelve una entidad dominio Tarjeta o null si no existe
        var tarjeta = await repositorio.ObtenerPorIdAsync(consulta.TarjetaId);

        //Paso 2: Si la tarjeta no existe, retornamos null
        if (tarjeta is null) return null;

        //Paso 3: Si la tarjeta existe, retornamos el DTO
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
            EstaActiva: tarjeta.EstaActiva,
            MesVencimiento: tarjeta.MesVencimiento,
            AnioVencimiento: tarjeta.AnioVencimiento,
            FechaVencimiento: tarjeta.FechaVencimiento,
            EstaVencida: tarjeta.EstaVencida,
            DiaCorte: tarjeta.DiaCorte,
            DiaPago: tarjeta.DiaPago,
            ProximoCorte: tarjeta.ProximoCorte,
            ProximoPago: tarjeta.ProximoPago,
            DiasParaCorte: tarjeta.DiasParaCorte,
            DiasParaPago: tarjeta.DiasParaPago
        );
    }
}