using FinanzasApp.Aplicacion.Tarjetas.Servicios;

namespace FinanzasApp.Aplicacion.DTOs;

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
