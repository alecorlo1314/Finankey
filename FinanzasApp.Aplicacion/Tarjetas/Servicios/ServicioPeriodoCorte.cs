using FinanzasApp.Domain.Entidades;

namespace FinanzasApp.Aplicacion.Tarjetas.Servicios;

public class ServicioPeriodoCorte
{
    public PeriodoCorte ObtenerPeriodoActual(int diaCorte)
    {
        var hoy = DateTime.Today;

        // Calcula el corte de este mes
        var diasEnMesActual = DateTime.DaysInMonth(hoy.Year, hoy.Month);
        var diaReal = Math.Min(diaCorte, diasEnMesActual);
        var corteMesActual = new DateTime(hoy.Year, hoy.Month, diaReal);

        DateTime inicioPeriodo;
        DateTime finPeriodo;

        if (hoy <= corteMesActual)
        {
            // Estamos antes del corte → el período comenzó el mes pasado
            var mesAnterior = hoy.AddMonths(-1);
            var diasMesAnterior = DateTime.DaysInMonth(mesAnterior.Year, mesAnterior.Month);
            var diaInicioAnterior = Math.Min(diaCorte, diasMesAnterior);

            inicioPeriodo = new DateTime(mesAnterior.Year, mesAnterior.Month, diaInicioAnterior)
                .AddDays(1);
            finPeriodo = corteMesActual;
        }
        else
        {
            // Ya pasó el corte → el período comenzó después del corte de este mes
            inicioPeriodo = corteMesActual.AddDays(1);

            var mesSiguiente = hoy.AddMonths(1);
            var diasMesSiguiente = DateTime.DaysInMonth(mesSiguiente.Year, mesSiguiente.Month);
            var diaCorteNext = Math.Min(diaCorte, diasMesSiguiente);
            finPeriodo = new DateTime(mesSiguiente.Year, mesSiguiente.Month, diaCorteNext);
        }

        return new PeriodoCorte(inicioPeriodo, finPeriodo, diaCorte);
    }
    /// <summary>
    /// Obtiene todos los períodos de los últimos N meses.
    /// Útil para mostrar el historial de estados de cuenta.
    /// </summary>
    public List<PeriodoCorte> ObtenerUltimosPeriodos(int diaCorte, int cantidad = 6)
    {
        var periodos = new List<PeriodoCorte>();
        var periodoActual = ObtenerPeriodoActual(diaCorte);

        periodos.Add(periodoActual);

        for (int i = 1; i < cantidad; i++)
        {
            var inicio = periodoActual.Inicio.AddMonths(-i);
            var fin = periodoActual.Fin.AddMonths(-i);
            periodos.Add(new PeriodoCorte(inicio, fin, diaCorte));
        }

        return periodos;
    }

    /// <summary>
    /// Filtra las transacciones que pertenecen a un período específico.
    /// </summary>
    public List<Transaccion> ObtenerTransaccionesDelPeriodo(
        IEnumerable<Transaccion> transacciones,
        PeriodoCorte periodo)
    {
        return transacciones
            .Where(t => t.Fecha.Date >= periodo.Inicio
                     && t.Fecha.Date <= periodo.Fin)
            .OrderByDescending(t => t.Fecha)
            .ToList();
    }
}