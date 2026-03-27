using FinanzasApp.Aplicacion.DTOs;
using FinanzasApp.Aplicacion.Interfaces;

namespace FinanzasApp.Aplicacion.Tarjetas.Consultas;

/// <summary>
/// Obtiene el detalle completo de una tarjeta
/// </summary>
/// <param name="TarjetaId"></param>
/// <returns>TarjetaResumenDto</returns>
public record ObtenerTarjetaPorIdConsulta(int TarjetaId) : IConsulta<TarjetaResumenDto?>;
