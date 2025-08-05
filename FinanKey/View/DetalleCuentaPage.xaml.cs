using FinanKey.ViewModels;

namespace FinanKey.View;

public partial class DetalleCuentaPage : ContentPage
{
	public DetalleCuentaPage(ViewModelDetalleCuenta viewModelDetalleCuenta)
	{
		InitializeComponent();
		BindingContext = viewModelDetalleCuenta;
    }

    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
    }
    private void Entry_Focused(object sender, FocusEventArgs e)
    {
        borderBarraBusqueda.Stroke = Color.Parse("#4E61B6");
        borderBarraBusqueda.StrokeThickness = 2;
    }

    private void Entry_Unfocused(object sender, FocusEventArgs e)
    {
        borderBarraBusqueda.Stroke = Colors.Transparent;
    }
}