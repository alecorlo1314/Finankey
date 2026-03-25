using FinanzasApp.Aplicacion.DTOs;
using FinanzasApp.Aplicacion.Interfaces;
using FinanzasApp.Domain.Entidades;
using FinanzasApp.Domain.Enumeraciones;
using FinanzasApp.Domain.Interfaces;

namespace FinanzasApp.Aplicacion.Tarjetas.Consultas;

// ── Consultas ─────────────────────────────────────────────────────────────────

/// <summary>Obtiene todas las tarjetas activas del usuario</summary>
public record ObtenerTarjetasConsulta : IConsulta<IEnumerable<TarjetaResumenDto>>;

/// <summary>Obtiene el detalle completo de una tarjeta</summary>
public record ObtenerTarjetaPorIdConsulta(int TarjetaId) : IConsulta<TarjetaResumenDto?>;

/// <summary>Obtiene el resumen financiero global para el dashboard</summary>
public record ObtenerResumenFinancieroConsulta(int Anio, int Mes) : IConsulta<ResumenFinancieroDto>;

// ── Manejadores ───────────────────────────────────────────────────────────────

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

public class ObtenerResumenFinancieroManejador(
    IRepositorioTarjeta repositorioTarjeta,
    IRepositorioTransaccion repositorioTransaccion)
    : IManejadorConsulta<ObtenerResumenFinancieroConsulta, ResumenFinancieroDto>
{
    public async Task<ResumenFinancieroDto> ManejarAsync(
        ObtenerResumenFinancieroConsulta consulta,
        CancellationToken cancellationToken = default)
    {
        var tarjetas = (await repositorioTarjeta.ObtenerActivasAsync()).ToList();

        // Suma de saldos positivos (débito = activos) y negativos (crédito = deuda)
        decimal totalActivos = tarjetas
            .Where(t => t.Tipo == TipoTarjeta.Debito)
            .Sum(t => t.SaldoActual);

        decimal totalDeudas = tarjetas
            .Where(t => t.Tipo == TipoTarjeta.Credito)
            .Sum(t => t.SaldoActual);

        // Gastos e ingresos del mes actual (suma de todas las tarjetas)
        decimal gastosMes = 0, ingresosMes = 0;
        var resumenCategorias = new Dictionary<CategoriaTransaccion, decimal>();

        foreach (var tarjeta in tarjetas)
        {
            gastosMes += await repositorioTransaccion.ObtenerTotalPorMesAsync(
                tarjeta.Id, consulta.Anio, consulta.Mes, TipoTransaccion.Gasto);

            ingresosMes += await repositorioTransaccion.ObtenerTotalPorMesAsync(
                tarjeta.Id, consulta.Anio, consulta.Mes, TipoTransaccion.Ingreso);

            // Acumula categorías de todas las tarjetas
            var categoriasTarjeta = await repositorioTransaccion
                .ObtenerResumenPorCategoriaAsync(tarjeta.Id, consulta.Anio, consulta.Mes);

            foreach (var (cat, monto) in categoriasTarjeta)
            {
                resumenCategorias.TryGetValue(cat, out var actual);
                resumenCategorias[cat] = actual + monto;
            }
        }

        // Paleta de colores para las categorías en gráficas
        var coloresCategorias = ObtenerColoresCategorias();

        var graficaCategorias = resumenCategorias
            .OrderByDescending(kvp => kvp.Value)
            .Select(kvp => new GraficaCategoriaDto(
                Categoria: kvp.Key,
                Monto: kvp.Value,
                Porcentaje: gastosMes > 0 ? (double)(kvp.Value / gastosMes) * 100 : 0,
                ColorHex: coloresCategorias.GetValueOrDefault(kvp.Key, "#94A3B8")
            ))
            .ToList();

        return new ResumenFinancieroDto(
            TotalActivos: totalActivos,
            TotalDeudas: totalDeudas,
            GastosMesActual: gastosMes,
            IngresosMesActual: ingresosMes,
            BalanceMes: ingresosMes - gastosMes,
            GastosPorCategoria: graficaCategorias
        );
    }

    /// <summary>
    /// Colores tranquilos (verde-azulados) para categorías en gráficas.
    /// Paleta inspirada en calma financiera.
    /// </summary>
    private static Dictionary<CategoriaTransaccion, string> ObtenerColoresCategorias() => new()
    {
        [CategoriaTransaccion.Alimentacion]   = "#2DD4BF",
        [CategoriaTransaccion.Transporte]      = "#38BDF8",
        [CategoriaTransaccion.Entretenimiento] = "#818CF8",
        [CategoriaTransaccion.Salud]           = "#34D399",
        [CategoriaTransaccion.Educacion]       = "#60A5FA",
        [CategoriaTransaccion.Hogar]           = "#A78BFA",
        [CategoriaTransaccion.Ropa]            = "#F472B6",
        [CategoriaTransaccion.Tecnologia]      = "#FB923C",
        [CategoriaTransaccion.Viajes]          = "#FACC15",
        [CategoriaTransaccion.Servicios]       = "#4ADE80",
        [CategoriaTransaccion.Restaurantes]    = "#F87171",
        [CategoriaTransaccion.Deportes]        = "#22D3EE",
        [CategoriaTransaccion.Suscripciones]   = "#C084FC",
        [CategoriaTransaccion.SalarioSueldo]   = "#86EFAC",
        [CategoriaTransaccion.Freelance]       = "#67E8F9",
        [CategoriaTransaccion.Inversiones]     = "#BEF264",
        [CategoriaTransaccion.Reembolsos]      = "#FDE68A",
        [CategoriaTransaccion.Otros]           = "#94A3B8",
    };
}
