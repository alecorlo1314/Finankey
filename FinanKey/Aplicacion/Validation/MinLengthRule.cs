namespace FinanKey.Aplicacion.Validation;

// Regla para longitud mínima
public class MinLengthRule<T> : IValidationRule<T>
{
    public int MinLength { get; set; }
    public string ValidationMessage { get; set; }

    public bool Check(T value)
    {
        if (value is string str)
        {
            return str.Length >= MinLength;
        }
        return false;
    }
}