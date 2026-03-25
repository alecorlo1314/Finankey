namespace FinanzasApp.Aplicacion.Tarjetas.Servicios;

public record PeriodoCorte(DateTime Inicio, DateTime Fin, int DiaCorte)
{
    /// <summary>
    /// Indica si este período ya cerró (el corte ya pasó).
    /// Un período cerrado tiene un monto fijo a pagar.
    /// </summary>
    public bool EstaCerrado => Fin < DateTime.Today;

    /// <summary>
    /// Indica si este es el período en curso (aún no ha llegado el corte).
    /// </summary>
    public bool EstaEnCurso => !EstaCerrado;

    /// <summary>
    /// Etiqueta legible del período.
    /// Ejemplo: "16 feb — 15 mar 2026"
    /// </summary>
    public string Etiqueta => 
        $"{Inicio:dd MMM} — {Fin:dd MMM yyyy}";
}
