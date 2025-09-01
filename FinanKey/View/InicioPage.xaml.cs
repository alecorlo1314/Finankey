
using FinanKey.ViewModels;

namespace FinanKey.View;

public partial class InicioPage : ContentPage
{
    //Inyeccion de dependencias
    private readonly ViewModelTarjeta _viewModelTarjeta;
    public InicioPage(ViewModelTarjeta _viewModelTarjeta)
	{
		InitializeComponent();
        //Vinculacion del contexto del viewModelTarjeta
        BindingContext = _viewModelTarjeta;
	}

    //Se cargan datos iniciales para la vista
    protected override void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext is ViewModelTarjeta vmTarjeta)
        {
            //Se cargan datos iniciales sobre las tarjetas que hay
            _ = vmTarjeta.ObtenerTarjetas();
        }
    }


    private void Entry_Focused(object sender, FocusEventArgs e)
    {
        //borderBarraBusqueda.Stroke = Color.Parse("#4f46e5");
        //borderBarraBusqueda.StrokeThickness = 2;
    }

    private void Entry_Unfocused(object sender, FocusEventArgs e)
    {
        //borderBarraBusqueda.Stroke = Colors.Transparent;
    }
}