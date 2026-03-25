using FinanzasApp.Aplicacion.DTOs;
using FinanzasApp.Aplicacion.Interfaces;
using FinanzasApp.Domain.Entidades;
using FinanzasApp.Domain.Enumeraciones;
using FinanzasApp.Domain.Interfaces;

namespace FinanzasApp.Aplicacion.Transacciones.Consultas;

// ── Consultas ─────────────────────────────────────────────────────────────────

public record ObtenerTransaccionesPorTarjetaConsulta(int TarjetaId, TipoTransaccion? FiltroTipo = null)
    : IConsulta<IEnumerable<TransaccionResumenDto>>;

public record ObtenerTransaccionPorIdConsultaUnParametro(int TransaccionId)
    : IConsulta<TransaccionResumenDto?>;

public record ObtenerTransaccionesRecientesConsulta(int Cantidad = 10)
    : IConsulta<IEnumerable<TransaccionResumenDto>>;

// ── Manejadores ───────────────────────────────────────────────────────────────

public class ObtenerTransaccionesPorTarjetaManejador(
    IRepositorioTransaccion repositorioTransaccion,
    IRepositorioTarjeta repositorioTarjeta)
    : IManejadorConsulta<ObtenerTransaccionesPorTarjetaConsulta, IEnumerable<TransaccionResumenDto>>
{
    public async Task<IEnumerable<TransaccionResumenDto>> ManejarAsync(
        ObtenerTransaccionesPorTarjetaConsulta consulta,
        CancellationToken cancellationToken = default)
    {
        //Si filtro activo es null, se muestran todas las transacciones
        var tarjeta = await repositorioTarjeta.ObtenerPorIdAsync(consulta.TarjetaId);
        var nombreTarjeta = tarjeta?.Nombre ?? "Tarjeta eliminada";

        IEnumerable<Transaccion> transacciones;

        //Si filtro activo es null, se muestran todas las transacciones
        if (consulta.FiltroTipo.HasValue)
        {
            transacciones = await repositorioTransaccion
                .ObtenerPorTarjetaYTipoAsync(consulta.TarjetaId, consulta.FiltroTipo.Value);
        }
        else
        {
            transacciones = await repositorioTransaccion
                .ObtenerPorTarjetaAsync(consulta.TarjetaId);
        }

        return transacciones
            .OrderByDescending(t => t.Fecha)
            .Select(t => MapearADto(t, nombreTarjeta));
    }

    private static TransaccionResumenDto MapearADto(Transaccion t, string nombreTarjeta) => new(
        Id: t.Id,
        TarjetaId: t.TarjetaId,
        NombreTarjeta: nombreTarjeta,
        Descripcion: t.Descripcion,
        Monto: t.Monto,
        Tipo: t.Tipo,
        Categoria: t.Categoria,
        CategoriaPredicha: t.CategoriaPredicha,
        ConfianzaPrediccion: t.ConfianzaPrediccion,
        Fecha: t.Fecha,
        Notas: t.Notas
    );
}

public class ObtenerTransaccionesRecientesManejador(
    IRepositorioTransaccion repositorioTransaccion,
    IRepositorioTarjeta repositorioTarjeta)
    : IManejadorConsulta<ObtenerTransaccionesRecientesConsulta, IEnumerable<TransaccionResumenDto>>
{
    public async Task<IEnumerable<TransaccionResumenDto>> ManejarAsync(
        ObtenerTransaccionesRecientesConsulta consulta,
        CancellationToken cancellationToken = default)
    {
        var transacciones = await repositorioTransaccion.ObtenerRecientesAsync(consulta.Cantidad);
        var tarjetas = (await repositorioTarjeta.ObtenerTodosAsync())
            .ToDictionary(t => t.Id, t => t.Nombre);

        return transacciones.Select(t => new TransaccionResumenDto(
            Id: t.Id,
            TarjetaId: t.TarjetaId,
            NombreTarjeta: tarjetas.GetValueOrDefault(t.TarjetaId, "Tarjeta eliminada"),
            Descripcion: t.Descripcion,
            Monto: t.Monto,
            Tipo: t.Tipo,
            Categoria: t.Categoria,
            CategoriaPredicha: t.CategoriaPredicha,
            ConfianzaPrediccion: t.ConfianzaPrediccion,
            Fecha: t.Fecha,
            Notas: t.Notas
        ));
    }
}
