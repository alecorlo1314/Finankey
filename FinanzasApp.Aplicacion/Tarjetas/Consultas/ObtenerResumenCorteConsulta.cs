using FinanzasApp.Aplicacion.DTOs;
using FinanzasApp.Aplicacion.Interfaces;

namespace FinanzasApp.Aplicacion.Tarjetas.Consultas;

/// <summary>
/// Obtiene el resumen del período de corte actual de una tarjeta de crédito.
/// Incluye las transacciones del período y el total a pagar.
/// </summary>
public record ObtenerResumenCorteConsulta(int TarjetaId)
    : IConsulta<ResumenCorteDto?>;
