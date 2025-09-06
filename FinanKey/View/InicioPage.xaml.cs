using FinanKey.Presentacion.ViewModels;

namespace FinanKey.View;

public partial class InicioPage : ContentPage
{
    //Inyección de dependencias
    private readonly ViewModelInicio _viewModelInicio;
    public InicioPage(ViewModelInicio viewModelInicio)
	{
		InitializeComponent();
        _viewModelInicio = viewModelInicio;
        BindingContext = viewModelInicio;
    }

    //Se cargan datos iniciales para la vista
    protected override void OnAppearing()
    {
        base.OnAppearing();

        // Llamada al ViewModelInicio
        _= _viewModelInicio.CargarTarjetasAsync();
    }
}