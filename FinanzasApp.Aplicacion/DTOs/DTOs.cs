using FinanzasApp.Aplicacion.Tarjetas.Servicios;
using FinanzasApp.Domain.Enumeraciones;

namespace FinanzasApp.Aplicacion.DTOs;

/// <summary>DTO para mostrar el resumen de una tarjeta en listas y dashboard</summary>
public record TarjetaResumenDto(
    int Id,
    string Nombre,
    string UltimosDigitos,
    TipoTarjeta Tipo,
    string ColorHex,
    string Banco,
    string RedTarjeta,
    decimal SaldoActual,
    decimal? LimiteCredito,
    decimal CreditoDisponible,
    double PorcentajeUso,
    bool EstaActiva
);

/// <summary>DTO para crear o editar una tarjeta</summary>
public record TarjetaFormDto(
    int? Id,                    // null para creación, valor para edición
    string Nombre,
    string UltimosDigitos,
    TipoTarjeta Tipo,
    string ColorHex,
    string Banco,
    string RedTarjeta,
    decimal? LimiteCredito,
    decimal SaldoActual
);

/// <summary>DTO para mostrar una transacción en listas</summary>
public record TransaccionResumenDto(
    int Id,
    int TarjetaId,
    string NombreTarjeta,
    string Descripcion,
    decimal Monto,
    TipoTransaccion Tipo,
    CategoriaTransaccion Categoria,
    bool CategoriaPredicha,
    float ConfianzaPrediccion,
    DateTime Fecha,
    string? Notas
);

/// <summary>DTO para crear o editar una transacción</summary>
public record TransaccionFormDto(
    int? Id,
    int TarjetaId,
    string Descripcion,
    decimal Monto,
    TipoTransaccion Tipo,
    CategoriaTransaccion Categoria,
    bool CategoriaPredicha,
    float ConfianzaPrediccion,
    DateTime Fecha,
    string? Notas
);

/// <summary>DTO para el resumen financiero del dashboard principal</summary>
public record ResumenFinancieroDto(
    decimal TotalActivos,           // Suma saldos tarjetas débito
    decimal TotalDeudas,            // Suma saldos tarjetas crédito
    decimal GastosMesActual,
    decimal IngresosMesActual,
    decimal BalanceMes,             // Ingresos - Gastos
    List<GraficaCategoriaDto> GastosPorCategoria
);

/// <summary>DTO para gráficas de distribución por categoría</summary>
public record GraficaCategoriaDto(
    CategoriaTransaccion Categoria,
    decimal Monto,
    double Porcentaje,
    string ColorHex               // Color para la gráfica
);

/// <summary>
/// Contiene toda la información del período de corte actual
/// de una tarjeta de crédito para mostrarlo en la UI.
/// </summary>
public record ResumenCorteDto(
    int TarjetaId,
    string NombreTarjeta,
    PeriodoCorte Periodo,
    decimal TotalPeriodo,       
    int CantidadMovimientos,
    DateTime? ProximoCorte,
    DateTime? ProximoPago,
    int? DiasParaCorte,
    int? DiasParaPago,
    decimal? LimiteCredito,
    decimal CreditoDisponible
);
