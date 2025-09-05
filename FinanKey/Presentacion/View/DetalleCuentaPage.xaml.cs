using FinanKey.Presentacion.ViewModels;

namespace FinanKey.Presentacion.View;

public partial class DetalleCuentaPage : ContentPage
{
    //Inyección de dependencias
    private readonly ViewModelDetalleCuenta _viewModelDetalleCuenta;
    public DetalleCuentaPage(ViewModelDetalleCuenta viewModelDetalleCuenta)
	{
		InitializeComponent();
        _viewModelDetalleCuenta = viewModelDetalleCuenta;
        BindingContext = _viewModelDetalleCuenta;
    }

    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
    }
    protected override void OnAppearing()
    {
        base.OnAppearing();
        // Cargar los datos iniciales
        _viewModelDetalleCuenta.CargaDatosInicialAsyn();
    }
    protected
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