using CommunityToolkit.Mvvm.ComponentModel;
using FinanKey.Presentacion.Intefaces;
using FinanKey.Presentacion.View.Intefaces;
namespace FinanKey.Presentacion.View.Validaciones;

public partial class ValidatableObject<T> : ObservableObject, IValidity
{
    #region PROPIEDADES
    [ObservableProperty]
    private IEnumerable<string> _errors;
    [ObservableProperty]
    private bool _isValid;
    [ObservableProperty]
    private T _value;

    //contiene las reglas de validacion
    public List<IValidationRule<T>> Validations { get; } = new();
    #endregion

    #region CONSTRUCTOR
    public ValidatableObject()
    {
        _isValid = true;
        _errors = Enumerable.Empty<string>();
    }
    #endregion

    #region METODO DE VALIDACION
    public bool Validate()
    {
        Errors = Validations
            ?.Where(v => !v.Check(Value))
            ?.Select(v => v.ValidationMessage)
            ?.ToArray()
            ?? Enumerable.Empty<string>();

        IsValid = !Errors.Any();
        return IsValid;
    }
    #endregion
}
