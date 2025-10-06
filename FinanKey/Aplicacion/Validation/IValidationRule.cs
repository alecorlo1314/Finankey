namespace FinanKey.Aplicacion.Validation;

// 1. Interfaz base para reglas de validación
public interface IValidationRule<T>
{
    string ValidationMessage { get; set; }
    bool Check(T value);
}
