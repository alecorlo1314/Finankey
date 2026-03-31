using FinanzasApp.Presentacion.ViewModels.Transacciones;

namespace FinanzasApp.Presentacion.Vistas.Transacciones;

public partial class TransaccionesGlobalPage : ContentPage
{
    private readonly MovimientosViewModel _viewModel;

    public TransaccionesGlobalPage(MovimientosViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.AlAparecerAsync();
    }
}
