using FinanzasApp.Aplicacion.Interfaces;
using FinanzasApp.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinanzasApp.Aplicacion.Tarjetas.Comandos.Manejadores;

/// <summary>
/// Actualiza una tarjeta existente. Valida que exista antes de modificar.
/// </summary>
public class ActualizarTarjetaManejador(IRepositorioTarjeta repositorio)
    : IManejadorComando<ActualizarTarjetaComando, bool>
{
    public async Task<bool> ManejarAsync(ActualizarTarjetaComando comando, CancellationToken cancellationToken = default)
    {
        var datos = comando.Datos;

        if (!datos.Id.HasValue)
            throw new ArgumentException("El Id de la tarjeta es requerido para actualizar.");

        var tarjetaExistente = await repositorio.ObtenerPorIdAsync(datos.Id.Value)
            ?? throw new KeyNotFoundException($"No se encontró la tarjeta con Id {datos.Id.Value}.");

        // Solo actualizamos los campos que el usuario puede modificar
        tarjetaExistente.Nombre = datos.Nombre.Trim();
        tarjetaExistente.UltimosDigitos = datos.UltimosDigitos;
        tarjetaExistente.Tipo = datos.Tipo;
        tarjetaExistente.ColorHex = datos.ColorHex;
        tarjetaExistente.Banco = datos.Banco.Trim();
        tarjetaExistente.RedTarjeta = datos.RedTarjeta;
        tarjetaExistente.LimiteCredito = datos.LimiteCredito;
        tarjetaExistente.SaldoActual = datos.SaldoActual;

        await repositorio.ActualizarAsync(tarjetaExistente);
        return true;
    }
}
