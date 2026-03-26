using FinanzasApp.Aplicacion.Interfaces;
using FinanzasApp.Domain.Entidades;
using FinanzasApp.Domain.Interfaces;

namespace FinanzasApp.Aplicacion.Tarjetas.Comandos.Manejadores;

/// <summary>
/// Crea una nueva tarjeta validando los datos y persistiéndola.
/// Retorna el Id asignado por la base de datos.
/// </summary>
public class CrearTarjetaManejador(IRepositorioTarjeta repositorio)
    : IManejadorComando<CrearTarjetaComando, int>
{
    public async Task<int> ManejarAsync(CrearTarjetaComando comando, CancellationToken cancellationToken = default)
    {
        var datos = comando.Datos;

        // Validación mínima de negocio
        if (string.IsNullOrWhiteSpace(datos.Nombre))
            throw new ArgumentException("El nombre de la tarjeta es requerido.");

        if (datos.UltimosDigitos.Length != 4 || !datos.UltimosDigitos.All(char.IsDigit))
            throw new ArgumentException("Los últimos dígitos deben ser exactamente 4 números.");

        var tarjeta = new Tarjeta
        {
            Nombre = datos.Nombre.Trim(),
            UltimosDigitos = datos.UltimosDigitos,
            Tipo = datos.Tipo,
            ColorHex = datos.ColorHex,
            Banco = datos.Banco.Trim(),
            RedTarjeta = datos.RedTarjeta,
            LimiteCredito = datos.LimiteCredito,
            SaldoActual = datos.SaldoActual,
            FechaRegistro = DateTime.Now,
            EstaActiva = true
        };

        return await repositorio.AgregarAsync(tarjeta);
    }
}
