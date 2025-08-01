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
}