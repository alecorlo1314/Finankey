namespace FinanKey.Aplicacion.Validation;

// Regla para validar números en rango
public class RangeRule<T> : IValidationRule<T> where T : IComparable<T>
{
    public T Min { get; set; }
    public T Max { get; set; }
    public string ValidationMessage { get; set; }

    public bool Check(T value)
    {
        return value.CompareTo(Min) >= 0 && value.CompareTo(Max) <= 0;
    }
}