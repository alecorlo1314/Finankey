using FinanzasApp.Aplicacion.DTOs;
using FinanzasApp.Aplicacion.Interfaces;

namespace FinanzasApp.Aplicacion.Tarjetas.Consultas;

/// <summary>Obtiene todas las tarjetas activas del usuario</summary>
public record ObtenerTarjetasConsulta : IConsulta<IEnumerable<TarjetaResumenDto>?>;
