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
    //Este metodo es para cambiar el color del borde de la barra de busqueda cuando se enfoca y se desenfoca
    private void Entry_Focused(object sender, FocusEventArgs e)
    {
        borderBarraBusqueda.Stroke = Color.Parse("#4E61B6");
        borderBarraBusqueda.StrokeThickness = 2;
    }
    //Este metodo es para cambiar el color del borde de la barra de busqueda cuando se desenfoca
    private void Entry_Unfocused(object sender, FocusEventArgs e)
    {
        borderBarraBusqueda.Stroke = Colors.Transparent;
    }
}