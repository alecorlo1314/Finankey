using FinanzasApp.Domain.Enumeraciones;
using System.Globalization;

namespace FinanzasApp.Presentacion.Convertidores;

/// <summary>
/// Convierte bool a uno de dos colores pasados como parámetro separado por '|'.
/// Uso XAML: ConverterParameter='#ColorTrue|#ColorFalse'
/// </summary>
public class BoolToColorConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var partes = (parameter as string ?? "").Split('|');

        if (partes.Length < 2) 
            return Colors.Transparent;

        var esVerdadero = value is bool b && b;
        var hex = esVerdadero ? partes[0] : partes[1];

        try
        {
            return Color.FromArgb(hex);
        }
        catch
        {
            return Colors.Transparent;
        }
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        throw new NotImplementedException();
}

/// <summary>
/// Convierte el TipoTransaccion en el color de FONDO del icono.
/// Ingreso → teal pastel | Gasto → rojo pastel
/// </summary>
public class TipoTransaccionFondoConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        value is TipoTransaccion tipo && tipo == TipoTransaccion.Ingreso
            ? Color.FromArgb("#CCFBF1")
            : Color.FromArgb("#FEE2E2");

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        throw new NotImplementedException();
}

/// <summary>
/// Convierte TipoTransaccion a texto corto para etiquetas.
/// </summary>
public class TipoTransaccionTextoConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        value is TipoTransaccion tipo
            ? tipo == TipoTransaccion.Ingreso ? "Ingreso" : "Gasto"
            : string.Empty;

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        throw new NotImplementedException();
}
