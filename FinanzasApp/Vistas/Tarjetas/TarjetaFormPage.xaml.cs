using FinanzasApp.Presentacion.ViewModels.Tarjetas;

namespace FinanzasApp.Presentacion.Vistas.Tarjetas;

public partial class TarjetaFormPage : ContentPage
{
    private readonly TarjetaFormViewModel _viewModel;

    public TarjetaFormPage(TarjetaFormViewModel viewModel)
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
