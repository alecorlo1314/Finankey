using FinanzasApp.Aplicacion.DTOs;
using FinanzasApp.Aplicacion.Interfaces;
using FinanzasApp.Aplicacion.Tarjetas.Servicios;
using FinanzasApp.Domain.Entidades;
using FinanzasApp.Domain.Enumeraciones;
using FinanzasApp.Domain.Interfaces;

namespace FinanzasApp.Aplicacion.Tarjetas.Consultas.Manejadores;

public class ObtenerResumenCorteManejador(
    IRepositorioTarjeta repositorioTarjeta,
    IRepositorioTransaccion repositorioTransaccion,
    ServicioPeriodoCorte servicioPeriodo)
    : IManejadorConsulta<ObtenerResumenCorteConsulta, ResumenCorteDto?>
{
    public async Task<ResumenCorteDto?> ManejarAsync(
        ObtenerResumenCorteConsulta consulta,
        CancellationToken cancellationToken = default)
    {
        //Ejemplo por pasos

        //Paso 1: Obtener la tarjeta por su ID
        //Esto devuelve una entidad dominio Tarjeta
        var tarjeta = await repositorioTarjeta.ObtenerPorIdAsync(consulta.TarjetaId);

        //Paso 2: Solo aplica para tarjetas de crédito con día de corte configurado
        //Si la tarjeta no existe o la tarjeta no es de tipo credito o la tarjeta no tiene dia de corte configur, entonces retorna null
        if (tarjeta is null || tarjeta.Tipo != TipoTarjeta.Credito || !tarjeta.DiaCorte.HasValue)
            return null;

        //Paso 3: Obtener el periodo actual
        //Esto nos devuelve un objeto de tipo PeriodoCorte con EstaCerrado, EstaEnCurso, Etiqueta(inicio-fin)
        var periodo = servicioPeriodo.ObtenerPeriodoActual(tarjeta.DiaCorte.Value);

        //Paso 4: Obtener todas las transacciones de la tarjeta
        //Estos nos devuelve una IEnumerable<Transaccion>
        var todasLasTransacciones = await repositorioTransaccion
            .ObtenerPorTarjetaAsync(tarjeta.Id);

        //Paso 5: Filtrar las transacciones que corresponden al periodo actual
        //Esto nos devuelve una IEnumerable<Transaccion> solo con las transacciones del periodo actual
        var transaccionesPeriodo = servicioPeriodo
            .ObtenerTransaccionesDelPeriodo(
            todasLasTransacciones,
            periodo);

        //Paso 6: Calcular el total del periodo
        //Esto nos devuelve un monto correspondiente a los gastos totales
        var totalPeriodo = transaccionesPeriodo
            .Where(t => t.Tipo == TipoTransaccion.Gasto)
            .Sum(t => t.Monto);

        //Paso 7: Devolver el resumen del periodo
        return new ResumenCorteDto(
            TarjetaId: tarjeta.Id,
            NombreTarjeta: tarjeta.Nombre,
            Periodo: periodo,
            TotalPeriodo: totalPeriodo,
            CantidadMovimientos: int.Parse(transaccionesPeriodo.Count().ToString()),
            ProximoCorte: tarjeta.ProximoCorte,
            ProximoPago: tarjeta.ProximoPago,
            DiasParaCorte: tarjeta.DiasParaCorte,
            DiasParaPago: tarjeta.DiasParaPago,
            LimiteCredito: tarjeta.LimiteCredito,
            CreditoDisponible: tarjeta.CreditoDisponible
        );
    }

    // Mapeo simple de DTO a entidad para el servicio
    //Esto no se uso
    private static Transaccion MapearAEntidad(TransaccionResumenDto dto) => new()
    {
        Id = dto.Id,
        TarjetaId = dto.TarjetaId,
        Descripcion = dto.Descripcion,
        Monto = dto.Monto,
        Tipo = dto.Tipo,
        Categoria = dto.Categoria,
        Fecha = dto.Fecha
    };
}