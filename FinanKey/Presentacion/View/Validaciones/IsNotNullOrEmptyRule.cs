
using FinanKey.Presentacion.Intefaces;

namespace FinanKey.Presentacion.View.Validaciones;

public class IsNotNullOrEmptyRule<T> : IValidationRule<T>
{
    public string ValidationMessage { get; set; }

    public bool Check(T value) =>
        value is string str && !string.IsNullOrWhiteSpace(str);
}

