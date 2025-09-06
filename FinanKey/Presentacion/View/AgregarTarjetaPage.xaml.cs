using FinanKey.Presentacion.ViewModels;

namespace FinanKey.Presentacion.View;

public partial class AgregarTarjetaPage : ContentPage
{
    public AgregarTarjetaPage(ViewModelTarjeta viewModelTarjeta)
	{
		InitializeComponent();
        BindingContext = viewModelTarjeta;
    }
    private void OnEntryFocused(Border border)
    {
        border.Stroke = App.Current?.Resources["ColorAzulMarino"] as Color;
        border.StrokeThickness = 2;
    }

    private void OnEntryUnfocused(Border border)
    {
        border.Stroke = Colors.Transparent;
    }

    private void entradaBancoCuenta_Focused(object sender, FocusEventArgs e) => OnEntryFocused(borderBancoCuenta);
    private void entradaBancoCuenta_Unfocused(object sender, FocusEventArgs e) => OnEntryUnfocused(borderBancoCuenta);
    private void entradaNombreTarjeta_Focused(object sender, FocusEventArgs e) => OnEntryFocused(borderNombreCuenta);
    private void entradaNombreTarjeta_Unfocused(object sender, FocusEventArgs e) => OnEntryUnfocused(borderNombreCuenta);

    private void entradaUltimosDigitos_Focused(object sender, FocusEventArgs e) => OnEntryFocused(borderUltimosDigitos);
    private void entradaUltimosDigitos_Unfocused(object sender, FocusEventArgs e) => OnEntryUnfocused(borderUltimosDigitos);

    private void entradaVencimiento_Focused(object sender, FocusEventArgs e) => OnEntryFocused(borderVencimiento);
    private void entradaVencimiento_Unfocused(object sender, FocusEventArgs e) => OnEntryUnfocused(borderVencimiento);

    private void entradaLimiteCredito_Focused(object sender, FocusEventArgs e)
    {

    }
    private void entradaDescripcion_Focused(object sender, FocusEventArgs e) => OnEntryFocused(borderDescripcion);

    private void entradaDescripcion_Unfocused(object sender, FocusEventArgs e) =>OnEntryUnfocused(borderDescripcion);
}