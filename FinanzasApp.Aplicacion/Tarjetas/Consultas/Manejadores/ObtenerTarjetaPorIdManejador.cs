using FinanzasApp.Aplicacion.DTOs;
using FinanzasApp.Aplicacion.Interfaces;
using FinanzasApp.Domain.Enumeraciones;
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
            DiaCorte: tarjeta.Tipo == TipoTarjeta.Credito ? tarjeta.DiaCorte : null,
            DiaPago: tarjeta.Tipo == TipoTarjeta.Credito ? tarjeta.DiaPago : null,
            ProximoCorte: tarjeta.Tipo == TipoTarjeta.Credito ? tarjeta.ProximoCorte : null,
            ProximoPago: tarjeta.Tipo == TipoTarjeta.Credito ? tarjeta.ProximoPago : null,
            DiasParaCorte: tarjeta.Tipo == TipoTarjeta.Credito ? tarjeta.DiasParaCorte : null,
            DiasParaPago: tarjeta.Tipo == TipoTarjeta.Credito ? tarjeta.DiasParaPago : null
        );
    }
}