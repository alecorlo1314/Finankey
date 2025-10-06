
namespace FinanKey.Aplicacion.Validation;
public class ReglaFechaTarjeta<T> : IValidationRule<T>
{
    public string ValidationMessage { get; set; }

    public bool Check(T value)
    {
        if(value is string str)
        {
            if (string.IsNullOrEmpty(str) || str.Length != 5 || !str.Contains('/'))
                return false;

            var partes = str.Split('/');
            if (partes.Length != 2)
                return false;

            return int.TryParse(partes[0], out int mes) && mes >= 1 && mes <= 12 &&
                   int.TryParse(partes[1], out int año) && año >= 0 && año <= 99;
        }
        return false;
    }
}
