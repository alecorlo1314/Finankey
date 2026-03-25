namespace FinanzasApp.Aplicacion.Interfaces;

/// <summary>
/// Interfaz base para todos los comandos (operaciones que modifican estado).
/// TRespuesta es el tipo de retorno del comando.
/// </summary>
public interface IComando<TRespuesta> { }

/// <summary>
/// Interfaz base para consultas (operaciones de solo lectura).
/// TRespuesta es el tipo de datos que retorna la consulta.
/// </summary>
public interface IConsulta<TRespuesta> { }

/// <summary>
/// Manejador de un comando específico.
/// Implementa la lógica de negocio para un comando dado.
/// </summary>
public interface IManejadorComando<TComando, TRespuesta>
    where TComando : IComando<TRespuesta>
{
    Task<TRespuesta> ManejarAsync(TComando comando, CancellationToken cancellationToken = default);
}

/// <summary>
/// Manejador de una consulta específica.
/// Solo lee datos, nunca modifica estado.
/// </summary>
public interface IManejadorConsulta<TConsulta, TRespuesta>
    where TConsulta : IConsulta<TRespuesta>
{
    Task<TRespuesta> ManejarAsync(TConsulta consulta, CancellationToken cancellationToken = default);
}

/// <summary>
/// Mediador central que despacha comandos y consultas a sus respectivos manejadores.
/// Desacopla los ViewModels de la lógica de negocio.
/// </summary>
public interface IMediator
{
    /// <summary>Envía un comando y espera su resultado</summary>
    Task<TRespuesta> EnviarAsync<TRespuesta>(IComando<TRespuesta> comando, CancellationToken ct = default);

    /// <summary>Ejecuta una consulta y retorna los datos solicitados</summary>
    Task<TRespuesta> ConsultarAsync<TRespuesta>(IConsulta<TRespuesta> consulta, CancellationToken ct = default);
}
