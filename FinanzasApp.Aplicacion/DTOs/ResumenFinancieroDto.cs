namespace FinanzasApp.Aplicacion.DTOs;

/// <summary>DTO para el resumen financiero del dashboard principal</summary>
public record ResumenFinancieroDto(
    decimal TotalActivos,           // Suma saldos tarjetas débito
    decimal TotalDeudas,            // Suma saldos tarjetas crédito
    decimal GastosMesActual,
    decimal IngresosMesActual,
    decimal BalanceMes,             // Ingresos - Gastos
    List<GraficaCategoriaDto> GastosPorCategoria
);
