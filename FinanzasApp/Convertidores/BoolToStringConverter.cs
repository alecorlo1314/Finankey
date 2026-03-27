using System.Globalization;

namespace FinanzasApp.Presentacion.Convertidores;

/// <summary>
/// Convierte bool a uno de dos strings según el parámetro 'TextoTrue|TextoFalso'.
/// Usado para el badge "Cerrado|En curso" del banner de corte.
/// </summary>
public class BoolToStringConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var partes = (parameter as string ?? "").Split('|');
        if (partes.Length < 2) return string.Empty;
        return value is bool b && b ? partes[0] : partes[1];
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        throw new NotImplementedException();
}