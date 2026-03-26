using FinanzasApp.Aplicacion.DTOs;
using FinanzasApp.Aplicacion.Interfaces;

namespace FinanzasApp.Aplicacion.Tarjetas.Consultas;

/// <summary>Obtiene el resumen financiero global para el dashboard</summary>
public record ObtenerResumenFinancieroConsulta(int Anio, int Mes) : IConsulta<ResumenFinancieroDto>;
