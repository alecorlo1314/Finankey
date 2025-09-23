using CommunityToolkit.Mvvm.ComponentModel;
using FinanKey.Presentacion.Intefaces;
using FinanKey.Presentacion.View.Intefaces;
namespace FinanKey.Presentacion.View.Validaciones;

public partial class ObjetoValidable<T> : ObservableObject, IValidable
{
    #region PROPIEDADES
    //contiene los errores de validacion
    [ObservableProperty]
    private IEnumerable<string> _listaErrores;

    //decide si el objetos es valido o no
    [ObservableProperty]
    private bool _esValido;

    [ObservableProperty]
    private T _value;

    //contiene las reglas de validacion
    public List<IReglaValidacion<T>> Validaciones { get; } = new();
    #endregion

    #region CONSTRUCTOR
    public ObjetoValidable()
    {
        EsValido = true;
        ListaErrores = Enumerable.Empty<string>();
    }
    #endregion

    #region METODO DE VALIDACION
    public bool Validar()
    {
        ListaErrores = Validaciones
            ?.Where(v => !v.Check(Value))
            ?.Select(v => v.ValidationMessage)
            ?.ToArray()
            ?? Enumerable.Empty<string>();
        EsValido = !ListaErrores.Any();
        return EsValido;
    }
    #endregion
}
