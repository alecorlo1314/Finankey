using FinanzasApp.Aplicacion.Interfaces;

namespace FinanzasApp.Infraestructura;

/// <summary>
/// Implementación del Mediador usando el contenedor de dependencias de MAUI.
/// Resuelve el manejador correcto para cada comando o consulta en tiempo de ejecución.
/// Esto desacopla completamente los ViewModels de la lógica de negocio.
/// </summary>
public class Mediador(IServiceProvider proveedorServicios) : IMediator
{
    public async Task<TRespuesta> EnviarAsync<TRespuesta>(
        IComando<TRespuesta> comando,
        CancellationToken ct = default)
    {
        // Determina el tipo de manejador requerido para este comando específico
        var tipoManejador = typeof(IManejadorComando<,>)
            .MakeGenericType(comando.GetType(), typeof(TRespuesta));

        var manejador = proveedorServicios.GetRequiredService(tipoManejador);

        // Invocación dinámica del método ManejarAsync del manejador resuelto
        var resultado = await (Task<TRespuesta>)tipoManejador
            .GetMethod(nameof(IManejadorComando<IComando<TRespuesta>, TRespuesta>.ManejarAsync))!
            .Invoke(manejador, [comando, ct])!;

        return resultado;
    }

    public async Task<TRespuesta> ConsultarAsync<TRespuesta>(
        IConsulta<TRespuesta> consulta,
        CancellationToken ct = default)
    {
        var tipoManejador = typeof(IManejadorConsulta<,>)
            .MakeGenericType(consulta.GetType(), typeof(TRespuesta));

        var manejador = proveedorServicios.GetRequiredService(tipoManejador);

        var resultado = await (Task<TRespuesta>)tipoManejador
            .GetMethod(nameof(IManejadorConsulta<IConsulta<TRespuesta>, TRespuesta>.ManejarAsync))!
            .Invoke(manejador, [consulta, ct])!;

        return resultado;
    }
}
