using SQLite;

namespace FinanzasApp.Infraestructura.Persistencia.Repositorios;

/// <summary>
/// Implementación concreta del repositorio de tarjetas usando SQLite-net.
/// </summary>
public class RepositorioTarjeta(ContextoBD contexto) : IRepositorioTarjeta
{
    private async Task<SQLiteAsyncConnection> ConexionAsync() =>
        await contexto.ObtenerConexionAsync();

    public async Task<Tarjeta?> ObtenerPorIdAsync(int id)
    {
        var db = await ConexionAsync();
        return await db.Table<Tarjeta>().Where(t => t.Id == id).FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<Tarjeta>> ObtenerTodosAsync()
    {
        var db = await ConexionAsync();
        return await db.Table<Tarjeta>().ToListAsync();
    }

    public async Task<IEnumerable<Tarjeta>> ObtenerActivasAsync()
    {
        var db = await ConexionAsync();
        return await db.Table<Tarjeta>().Where(t => t.EstaActiva).ToListAsync();
    }

    public async Task<IEnumerable<Tarjeta>> ObtenerPorTipoAsync(TipoTarjeta tipo)
    {
        var db = await ConexionAsync();
        return await db.Table<Tarjeta>()
            .Where(t => t.EstaActiva && t.Tipo == tipo)
            .ToListAsync();
    }

    public async Task<int> AgregarAsync(Tarjeta entidad)
    {
        var db = await ConexionAsync();
        await db.InsertAsync(entidad);
        return entidad.Id;   // SQLite-net asigna el Id después del Insert
    }

    public async Task ActualizarAsync(Tarjeta entidad)
    {
        var db = await ConexionAsync();
        await db.UpdateAsync(entidad);
    }

    public async Task ActualizarSaldoAsync(int tarjetaId, decimal nuevoSaldo)
    {
        var db = await ConexionAsync();
        // Actualización parcial eficiente: solo modifica el campo saldo
        await db.ExecuteAsync(
            "UPDATE Tarjeta SET SaldoActual = ? WHERE Id = ?",
            nuevoSaldo, tarjetaId);
    }

    public async Task EliminarAsync(int id)
    {
        var db = await ConexionAsync();
        await db.DeleteAsync<Tarjeta>(id);
    }
}

/// <summary>
/// Implementación concreta del repositorio de transacciones usando SQLite-net.
/// </summary>
public class RepositorioTransaccion(ContextoBD contexto) : IRepositorioTransaccion
{
    private async Task<SQLiteAsyncConnection> ConexionAsync() =>
        await contexto.ObtenerConexionAsync();

    public async Task<Transaccion?> ObtenerPorIdAsync(int id)
    {
        var db = await ConexionAsync();
        return await db.Table<Transaccion>().Where(t => t.Id == id).FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<Transaccion>> ObtenerTodosAsync()
    {
        var db = await ConexionAsync();
        return await db.Table<Transaccion>().ToListAsync();
    }

    public async Task<IEnumerable<Transaccion>> ObtenerPorTarjetaAsync(int tarjetaId)
    {
        var db = await ConexionAsync();
        return await db.Table<Transaccion>()
            .Where(t => t.TarjetaId == tarjetaId)
            .OrderByDescending(t => t.Fecha)
            .ToListAsync();
    }

    public async Task<IEnumerable<Transaccion>> ObtenerPorRangoFechaAsync(DateTime inicio, DateTime fin)
    {
        var db = await ConexionAsync();
        return await db.Table<Transaccion>()
            .Where(t => t.Fecha >= inicio && t.Fecha <= fin)
            .OrderByDescending(t => t.Fecha)
            .ToListAsync();
    }

    public async Task<IEnumerable<Transaccion>> ObtenerPorTarjetaYTipoAsync(int tarjetaId, TipoTransaccion tipo)
    {
        var db = await ConexionAsync();
        return await db.Table<Transaccion>()
            .Where(t => t.TarjetaId == tarjetaId && t.Tipo == tipo)
            .OrderByDescending(t => t.Fecha)
            .ToListAsync();
    }

    public async Task<decimal> ObtenerTotalPorMesAsync(int tarjetaId, int anio, int mes, TipoTransaccion tipo)
    {
        var db = await ConexionAsync();
        var inicio = new DateTime(anio, mes, 1);
        var fin = inicio.AddMonths(1).AddTicks(-1);

        var transacciones = await db.Table<Transaccion>()
            .Where(t => t.TarjetaId == tarjetaId
                     && t.Tipo == tipo
                     && t.Fecha >= inicio
                     && t.Fecha <= fin)
            .ToListAsync();

        return transacciones.Sum(t => t.Monto);
    }

    public async Task<IEnumerable<Transaccion>> ObtenerRecientesAsync(int cantidad = 10)
    {
        var db = await ConexionAsync();
        return await db.Table<Transaccion>()
            .OrderByDescending(t => t.Fecha)
            .Take(cantidad)
            .ToListAsync();
    }

    public async Task<Dictionary<CategoriaTransaccion, decimal>> ObtenerResumenPorCategoriaAsync(
        int tarjetaId, int anio, int mes)
    {
        var db = await ConexionAsync();
        var inicio = new DateTime(anio, mes, 1);
        var fin = inicio.AddMonths(1).AddTicks(-1);

        var gastos = await db.Table<Transaccion>()
            .Where(t => t.TarjetaId == tarjetaId
                     && t.Tipo == TipoTransaccion.Gasto
                     && t.Fecha >= inicio
                     && t.Fecha <= fin)
            .ToListAsync();

        // Agrupa en memoria (SQLite-net no soporta GroupBy con proyección compleja)
        return gastos
            .GroupBy(t => t.Categoria)
            .ToDictionary(g => g.Key, g => g.Sum(t => t.Monto));
    }

    public async Task<int> AgregarAsync(Transaccion entidad)
    {
        var db = await ConexionAsync();
        await db.InsertAsync(entidad);
        return entidad.Id;
    }

    public async Task ActualizarAsync(Transaccion entidad)
    {
        var db = await ConexionAsync();
        await db.UpdateAsync(entidad);
    }

    public async Task EliminarAsync(int id)
    {
        var db = await ConexionAsync();
        await db.DeleteAsync<Transaccion>(id);
    }
}
