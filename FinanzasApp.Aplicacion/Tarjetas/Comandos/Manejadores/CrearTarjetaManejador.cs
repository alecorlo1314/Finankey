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
        //Paso 1: Validación de datos
        //Esto obtiene los datos del comando
        var datos = comando.Datos;

        //Paso 2: Validación de negocio
        // Validación mínima de negocio
        if (string.IsNullOrWhiteSpace(datos.Nombre))
            throw new ArgumentException("El nombre de la tarjeta es requerido.");

        if (datos.UltimosDigitos.Length != 4 || !datos.UltimosDigitos.All(char.IsDigit))
            throw new ArgumentException("Los últimos dígitos deben ser exactamente 4 números.");

        //Paso 3: Mapeo a entidad para guardar en DB
        var tarjeta = new Tarjeta
        {
            Nombre = datos.Nombre.Trim(),
            Banco = datos.Banco.Trim(),
            UltimosDigitos = datos.UltimosDigitos,
            MesVencimiento = datos.MesVencimiento,
            AnioVencimiento = datos.AnioVencimiento,
            Tipo = datos.Tipo,
            ColorHex = datos.ColorHex,
            RedTarjeta = datos.RedTarjeta,
            LimiteCredito = datos.LimiteCredito,
            SaldoActual = datos.SaldoActual,
            DiaCorte = datos.DiaCorte,
            DiaPago = datos.DiaPago,
            FechaRegistro = DateTime.Now,
            EstaActiva = true
        };

        //Paso 4: Guardar en DB
        //Retorna un 1 si se guardó con exito, de lo contrario 0 sino se pudo
        return await repositorio.AgregarAsync(tarjeta);
    }
}
