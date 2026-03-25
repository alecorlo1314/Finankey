using FinanzasApp.Domain.Entidades;
using FinanzasApp.Domain.Enumeraciones;

namespace FinanzasApp.Domain.Interfaces;

/// <summary>
/// Contrato base genérico para repositorios.
/// Define las operaciones CRUD mínimas esperadas en cualquier repositorio.
/// </summary>
public interface IRepositorioBase<T> where T : class
{
    Task<T?> ObtenerPorIdAsync(int id);
    Task<IEnumerable<T>> ObtenerTodosAsync();
    Task<int> AgregarAsync(T entidad);
    Task ActualizarAsync(T entidad);
    Task EliminarAsync(int id);
}

/// <summary>
/// Repositorio específico para tarjetas.
/// Extiende el base con consultas propias del dominio de tarjetas.
/// </summary>
public interface IRepositorioTarjeta : IRepositorioBase<Tarjeta>
{
    /// <summary>Obtiene solo las tarjetas activas del usuario</summary>
    Task<IEnumerable<Tarjeta>> ObtenerActivasAsync();

    /// <summary>Obtiene tarjetas filtradas por tipo (crédito o débito)</summary>
    Task<IEnumerable<Tarjeta>> ObtenerPorTipoAsync(TipoTarjeta tipo);

    /// <summary>Actualiza únicamente el saldo de la tarjeta (operación frecuente)</summary>
    Task ActualizarSaldoAsync(int tarjetaId, decimal nuevoSaldo);
}

/// <summary>
/// Repositorio específico para transacciones.
/// Incluye consultas de análisis y filtrado por diferentes criterios.
/// </summary>
public interface IRepositorioTransaccion : IRepositorioBase<Transaccion>
{
    /// <summary>Obtiene todas las transacciones de una tarjeta específica</summary>
    Task<IEnumerable<Transaccion>> ObtenerPorTarjetaAsync(int tarjetaId);

    /// <summary>Filtra transacciones por rango de fechas</summary>
    Task<IEnumerable<Transaccion>> ObtenerPorRangoFechaAsync(DateTime inicio, DateTime fin);

    /// <summary>Filtra por tarjeta y tipo (gastos o ingresos)</summary>
    Task<IEnumerable<Transaccion>> ObtenerPorTarjetaYTipoAsync(int tarjetaId, TipoTransaccion tipo);

    /// <summary>Calcula el total de gastos/ingresos de una tarjeta en un mes</summary>
    Task<decimal> ObtenerTotalPorMesAsync(int tarjetaId, int anio, int mes, TipoTransaccion tipo);

    /// <summary>Obtiene las transacciones más recientes para el dashboard</summary>
    Task<IEnumerable<Transaccion>> ObtenerRecientesAsync(int cantidad = 10);

    /// <summary>Agrupa gastos por categoría para gráficas</summary>
    Task<Dictionary<CategoriaTransaccion, decimal>> ObtenerResumenPorCategoriaAsync(
        int tarjetaId, int anio, int mes);
}
