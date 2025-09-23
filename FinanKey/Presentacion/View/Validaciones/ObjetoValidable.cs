using CommunityToolkit.Mvvm.ComponentModel;
using static System.Runtime.InteropServices.JavaScript.JSType;
namespace FinanKey.Presentacion.View.Validaciones;

class ObjetoValidable<T> : ObservableObject, IValidity
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
    public List<ReglasValidacion<T>> Validaciones { get; } = new();
    #endregion

    #region CONSTRUCTOR
    public ObjetoValidable()
    {
        EsValido = true;
        ListaErrores = new Enumerable.Empty<string>();
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
