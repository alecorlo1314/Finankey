using FinanzasApp.Aplicacion.DTOs;

namespace FinanzasApp.Presentacion.ViewModels.Tarjetas;

public class GrupoTarjetas : List<TarjetaResumenDto>
{
    /// <summary>
    /// Etiqueta del grupo
    /// Ejemplo (Debito,Credito)
    /// </summary>
    public string TipoTarjeta { get; }

    /// <summary>
    /// Cantidad de tarjetas en el grupo
    /// Ejemplo (2,3)
    /// </summary>
    public string Etiqueta { get; }

    /// <summary>
    /// Subtitulo del grupo
    /// Ejemplo (tarjetas, tarjetas)
    /// </summary>
    public string Subtitulo { get; }

    public GrupoTarjetas(
        string tipoTarjeta, 
        string etiqueta, 
        string subtitulo, 
        List<TarjetaResumenDto> tarjetas)
        : base(tarjetas)
    {
        TipoTarjeta = tipoTarjeta;
        Etiqueta = etiqueta;
        Subtitulo = subtitulo;
    }
}
