

namespace FinanKey.Aplicacion.Validation;

// Regla para campos no vacíos
public class IsNotNullOrEmptyRule<T> : IValidationRule<T>
{
    public string ValidationMessage { get; set; }

    public bool Check(T value) =>
        value is string str && !string.IsNullOrWhiteSpace(str);
}
