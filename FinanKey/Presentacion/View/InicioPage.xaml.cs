using CommunityToolkit.Maui.Core;
using FinanKey.Presentacion.ViewModels;

namespace FinanKey.Presentacion.View;

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
        _= _viewModelInicio.CargarMovimientosAsync();
    }
    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
        CommunityToolkit.Maui.Core.Platform.StatusBar.SetColor(App.Current?.Resources["ColorAzulPricipal"] as Color);
        CommunityToolkit.Maui.Core.Platform.StatusBar.SetStyle(StatusBarStyle.LightContent);
    }
}