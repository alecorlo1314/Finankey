using CommunityToolkit.Mvvm.ComponentModel;
using FinanzasApp.Domain.Enumeraciones;

namespace FinanzasApp.Presentacion.Modelos;

/// <summary>
/// Modelo para cada chip de categoría en el formulario.
/// Tener IsSeleccionada como propiedad observable permite
/// que el VSM reaccione directamente sin converters complejos.
/// </summary>
public partial class CategoriaItem : ObservableObject
{
    public CategoriaTransaccion Categoria { get; init; }
    public string Icono { get; init; } = string.Empty;      // Emoji o imagen
    public string Nombre { get; init; } = string.Empty;

    [ObservableProperty]
    private bool _estaSeleccionada;
}
