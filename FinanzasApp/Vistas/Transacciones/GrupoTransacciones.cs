// Crea este archivo en Presentacion/ViewModels/Transacciones/
using FinanzasApp.Aplicacion.DTOs;

namespace FinanzasApp.Presentacion.ViewModels.Transacciones;

/// <summary>
/// Representa un grupo de transacciones bajo una etiqueta de fecha.
/// Hereda de List para que CollectionView lo trate como colección agrupada.
/// </summary>
public class GrupoTransacciones : List<TransaccionResumenDto>
{
    /// <summary>
    /// Etiqueta del grupo: "Hoy", "Ayer", "Esta semana", "Este mes",
    /// o el nombre del mes anterior (ej: "Febrero 2026").
    /// </summary>
    public string Etiqueta { get; }

    /// <summary>
    /// Subtítulo opcional que muestra el total del grupo.
    /// Ej: "3 movimientos · ₡45,200"
    /// </summary>
    public string Subtitulo { get; }

    public GrupoTransacciones(
        string etiqueta,
        string subtitulo,
        IEnumerable<TransaccionResumenDto> items) : base(items)
    {
        Etiqueta = etiqueta;
        Subtitulo = subtitulo;
    }
}