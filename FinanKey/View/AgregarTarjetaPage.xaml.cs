using FinanKey.ViewModels;

namespace FinanKey.View;

public partial class AgregarTarjetaPage : ContentPage
{
    //Inyección de dependencias para los ViewModels
    private readonly ViewModelCuenta _viewModelCuenta;
    public AgregarTarjetaPage(ViewModelCuenta viewModelCuenta)
	{
		InitializeComponent();
        _viewModelCuenta = viewModelCuenta;
    }
}