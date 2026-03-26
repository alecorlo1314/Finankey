using FinanzasApp.Aplicacion.DTOs;
using FinanzasApp.Aplicacion.Interfaces;

namespace FinanzasApp.Aplicacion.Tarjetas.Comandos;

/// <summary>Comando para actualizar los datos de una tarjeta existente</summary>
public record ActualizarTarjetaComando(TarjetaFormDto Datos) : IComando<bool>;
