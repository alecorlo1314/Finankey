using FinanzasApp.Aplicacion.DTOs;
using FinanzasApp.Aplicacion.Interfaces;

namespace FinanzasApp.Aplicacion.Tarjetas.Comandos;

/// <summary>Comando para crear una nueva tarjeta</summary>
public record CrearTarjetaComando(TarjetaFormDto Datos) : IComando<int>;
